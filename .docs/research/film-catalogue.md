# Which film catalogue to use

Research for [#3](https://github.com/jenkinsonc/ai-demo/issues/3), a child of the map [#2 Film tracker v1](https://github.com/jenkinsonc/ai-demo/issues/2). Context: a local-only, single-user, non-commercial app that logs **Watches** of **Films** into a **Diary** (see `GLOSSARY.md`). It runs only on the owner's machine via Aspire dev mode and is never deployed.

Sources were read on 2026-11-22. Quotes are from the linked first-party pages.

## Recommendation

**Use TMDB (The Movie Database) API v3.** It is the only candidate that is explicitly licensed for free non-commercial use, returns every field the app needs (title, year, poster, director, runtime) in one details call, has decent fuzzy title search with a year filter, serves posters itself, and allows local caching for up to 6 months. The costs are an attribution obligation (TMDB logo plus a fixed notice in an About/Credits area) and a cache-refresh rule that bears directly on [#6](https://github.com/jenkinsonc/ai-demo/issues/6).

OMDb is the runner-up: licensed CC BY-NC 4.0 and simpler (director and runtime come back flat in one call), but posters are a patron-only feature, the free tier is 1,000 requests a day, and its terms forbid archiving content, which works against storing Film details locally.

## Comparison

| | TMDB | OMDb | IMDb non-commercial datasets | Wikidata |
|---|---|---|---|---|
| Personal, non-commercial terms | Free for non-commercial use with attribution | CC BY-NC 4.0; "personal, non-commercial purposes" | "personal and non-commercial use" | CC0, any use |
| API key | Free, from account settings | Free key emailed on sign-up | None (bulk download) | None |
| Search | `/search/movie` with `query`, `year`, `primary_release_year` | `s=` (list) or `t=` (single best match), plus `y=` | None: you build your own index from TSV files | SPARQL / search API, general-purpose |
| Title, year | Yes | Yes | Yes | Yes |
| Poster | Yes, served from `image.tmdb.org` | Patron-only Poster API | No images | Rarely; film posters are usually non-free |
| Director | Via `credits` (appendable to details) | Yes, flat field | Yes (`title.crew`) | Yes, as statements |
| Runtime | Yes (details) | Yes | Yes (`title.basics`) | Yes, as statements |
| Rate limit | ~40 requests/second | 1,000/day on free key | n/a | Fair-use |
| Local caching/storage | Allowed up to 6 months | Terms forbid archiving | Whole dataset is local by design | Unrestricted |

IMDb datasets and Wikidata are listed for completeness. Neither gives posters, and both push search-index work into the app, so neither is a serious fit for v1.

## TMDB

### Terms for a personal, non-commercial app

- The developer FAQ: "Our API is free to use for non-commercial purposes as long as you attribute TMDB as the source of the data and/or images." ([FAQ](https://developer.themoviedb.org/docs/faq))
- The [API Terms of Use](https://www.themoviedb.org/api-terms-of-use) (last updated October 20, 2023) grant a "non-exclusive, non-transferable, non-sublicensable" licence (section 1.A), conditional on "Giving TMDB attribution for all TMDB Content" (section 1.B). Section 2.A: the licence "does not permit any commercial use"; commercial use needs a separate written agreement.
- Section 2.A's examples of commercial use are charging for or selling an app that uses TMDB content, driving traffic or revenue to a destination site, search engine or chatbot, and training or validating AI/ML systems on TMDB content. A single-user local Diary does none of these.

### Getting an API key

- Register a TMDB account, then "click the API link from within your account settings page" and agree to the terms of use before a key is issued. Use a desktop browser. ([Getting started](https://developer.themoviedb.org/docs/getting-started))
- Two auth forms: a v3 `api_key` query parameter, or a read access token "sent along as an `Authorization` header". ([Authentication](https://developer.themoviedb.org/docs/authentication-application)) The Bearer token keeps the secret out of URLs and logs. Store it in .NET user-secrets or Aspire parameters, not in the repo.

### Search quality

- `GET /3/search/movie` takes `query` (required), `year`, `primary_release_year`, `include_adult` (default `false`), `language` (default `en-US`), `page` and `region`. ([Search movie](https://developer.themoviedb.org/reference/search-movie))
- Results are paged and carry `id`, `title`, `original_title`, `release_date`, `poster_path`, `overview`, `popularity` and vote fields. This is enough to render a pick-list with year and thumbnail.
- The search results carry **no director or runtime**. Those need a details call on the chosen Film.
- The search reference does not specify how partial or misspelt titles are matched. That has to be checked by hand with real queries before relying on it.

### Returned data

- `GET /3/movie/{movie_id}` returns `title`, `release_date`, `runtime` (minutes), `poster_path` and `imdb_id`, and "supports using `append_to_response`". ([Movie details](https://developer.themoviedb.org/reference/movie-details))
- `append_to_response` appends sub-requests to a detail call, e.g. `/3/movie/11?append_to_response=videos,images`; "Each request will get appended to the response as a new JSON object." ([Append to response](https://developer.themoviedb.org/docs/append-to-response)) So `?append_to_response=credits` returns the crew in the same call, and the director is the crew member whose `job` is `Director`. The `credits` sub-request follows the documented mechanism, but the reference pages read here do not show it by name. Confirm it with one live call.
- Year comes from `release_date` (ISO date); the app derives it.

### Posters and attribution

- Image URLs are built from a base URL, a size and the `poster_path`, e.g. `https://image.tmdb.org/t/p/w500/1E5baAaEse26fej7uHcjOgEE2t2.jpg`. The base URL and sizes come from `/configuration`. ([Image basics](https://developer.themoviedb.org/docs/image-basics))
- Attribution (Terms section 3): "You must use the TMDB logo to identify Your use of TMDB, the TMDB APIs, or TMDB Content", and you must place prominently the notice "This [website, program, service, application, product] uses TMDB and the TMDB APIs but is not endorsed, certified, or otherwise approved by TMDB." The FAQ says the attribution goes in the app's "About" or "Credits" type section. ([Terms](https://www.themoviedb.org/api-terms-of-use), [FAQ](https://developer.themoviedb.org/docs/faq))
- Terms section 1.C forbids using TMDB "as an image hosting service for banner advertisements, graphics, etc." Showing a Film's poster next to its Watch is ordinary use of TMDB Content.
- The FAQ notes that TMDB does not claim ownership of the images and data. Posters stay the rights-holders' material, which is one more reason the app should stay private and non-commercial.

### Rate limits

- Current limits "sit somewhere in the 40 requests per second range" and "could change at any time". The legacy 40-requests-per-10-seconds limit is disabled. Clients must "respect the `429`". ([Rate limiting](https://developer.themoviedb.org/docs/rate-limiting))
- The Terms set no daily quota but forbid uses that consume "an excessive amount of bandwidth" (section 1.C). A single user typing searches will never come close.

### Caching and storing results locally (input to [#6](https://github.com/jenkinsonc/ai-demo/issues/6))

- Terms section 1.C prohibits caching "for longer than 6 months, any information obtained through or from TMDB or the TMDB APIs." Caching for six months or less is therefore allowed. ([Terms](https://www.themoviedb.org/api-terms-of-use))
- Terms section 1.D: on termination "you must promptly delete or otherwise purge all TMDB Content, including any cached content."
- What #6 has to take from this:
  - **Always safe to store permanently:** the TMDB movie `id`, the app's own Watch data (date) and anything the owner types. The `id` is the Film's identity.
  - **Allowed, with refresh:** a local copy of title, year, runtime, director and `poster_path`, provided each copy is refreshed (re-fetched) or dropped within 6 months of fetching. Store a `fetched at` timestamp per Film.
  - **Poster image bytes** are TMDB Content too. If they are downloaded rather than hotlinked from `image.tmdb.org`, the same 6-month limit applies.
  - Copying details locally with periodic refresh is compliant; so is fetching each time. A copy that is stored forever and never refreshed is not.

## OMDb

- **Terms:** "All content licensed under CC BY-NC 4.0." ([OMDb home](https://www.omdbapi.com/)) The [Terms](https://www.omdbapi.com/legal.htm) (last updated March 12, 2015) grant use "solely for personal, non-commercial purposes" (4.1), say "You may not build a business utilizing the Contributions, whether or not for profit" (4.2.5), and require you to "retain all trademark, copyright and other proprietary notices" (4.2.1).
- **API key:** free key ("FREE! (1,000 daily limit)") emailed after entering an address. Patreon supporters get a patron key. ([API key page](https://www.omdbapi.com/apikey.aspx))
- **Search:** `s=` (required for list search) with `type`, `y`, `page` (1-100); `t=` returns a single title match; `i=` looks up by IMDb ID. The page does not say how fuzzy `s=` matching is or how many results come back per page. ([OMDb home](https://www.omdbapi.com/))
- **Data:** title lookups return flat fields including Title, Year, Runtime, Director and a Poster URL. The parameter docs on the home page show no example response, so these field names come from the API's known response shape and need checking with a live call.
- **Posters:** "The Poster API is only available to patrons." ([OMDb home](https://www.omdbapi.com/))
- **Rate limit:** 1,000 requests a day on the free key. The terms forbid creating "an undue burden" (2.8).
- **Caching/storage:** the terms do not mention caching, but 4.2.3 forbids you to "copy, download, reproduce, duplicate, archive, distribute" Contributions, and 4.2.6 forbids derived works. A strict reading rules out keeping Film details locally, so OMDb leans #6 towards fetching each time.
- The site is "not endorsed by or affiliated with IMDb.com".

## IMDb non-commercial datasets

- "Subsets of IMDb data are available for access to customers for personal and non-commercial use"; files are `title.basics`, `title.crew`, `title.principals`, `title.akas`, `title.ratings`, `title.episode` and `name.basics` (gzipped TSV), "refreshed daily". No images are described. ([IMDb datasets](https://data.imdb.com/non-commercial-datasets/))
- Covers title, year, runtime and director, but the app would have to download and index several large files and would still have no posters. That is too much infrastructure for v1.

## Wikidata

- Structured data "is made available under the Creative Commons CC0 License". ([Wikidata:Licensing](https://www.wikidata.org/wiki/Wikidata:Licensing)) The licence is the least restrictive of all the candidates and puts no limits on storage.
- Film items carry director and duration as statements, but querying needs SPARQL, title search is general-purpose rather than film-tuned, and film posters are usually non-free, so they are rarely available. Not a fit as the primary catalogue.

## Open points to check by hand

1. Partial and misspelt title matching on TMDB `/search/movie` (and OMDb `s=`) is not documented. Try a few real queries.
2. Confirm that `append_to_response=credits` on `/3/movie/{id}` returns the crew with `job: "Director"` in one call.
