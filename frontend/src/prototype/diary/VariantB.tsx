// PROTOTYPE - throwaway. Variant B "Poster wall": grouped by year, posters lead, rewatch badge,
// per-Film view is a modal. Search opens from a floating "+" button; picking a Film moves the
// same popup to a date step (default today, no future dates), then Save scrolls to and highlights the new Watch.
import { useEffect, useState } from 'react';
import { filmById, fmt, ordinal, todayIso, watchNumber, watchesOfFilm, newestFirst, highlightStyle, type Film, type Watch } from './mockData';
import { Poster, StubSearch } from './shared';
import type { VariantProps } from './types';

export function VariantB({ watches, onLog, highlightId }: VariantProps) {
  // Screenshot hooks: &demo=date opens the date step; &demo=saved logs a past-dated Watch on load.
  const demo = new URLSearchParams(location.search).get('demo');
  const [film, setFilm] = useState<Film | null>(null);
  const [searching, setSearching] = useState(demo === 'date');
  const [picked, setPicked] = useState<Film | null>(demo === 'date' ? filmById(4) : null);
  const [date, setDate] = useState(demo === 'date' ? '2025-03-14' : todayIso());
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (demo === 'saved') onLog(filmById(12), '2025-02-08');
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (highlightId === null) return;
    document.querySelector(`[data-watch-id="${highlightId}"]`)?.scrollIntoView({ block: 'center' });
  }, [highlightId]);

  const closeAll = () => { setFilm(null); setSearching(false); setPicked(null); setError(null); };
  const save = () => {
    if (!picked) return;
    const err = onLog(picked, date);
    if (err) setError(err); else closeAll();
  };
  const sorted = [...watches].sort(newestFirst);
  const years: { year: string; items: Watch[] }[] = [];
  for (const w of sorted) {
    const y = w.date.slice(0, 4);
    const last = years[years.length - 1];
    if (last?.year === y) last.items.push(w); else years.push({ year: y, items: [w] });
  }

  return (
    <div style={{ width: '100%' }}>
      {years.map((g) => (
        <section key={g.year} style={{ marginBottom: '2rem' }}>
          <h2 style={{ fontSize: '2rem', margin: '0 0 0.75rem', fontWeight: 800 }}>
            {g.year} <span style={{ fontSize: '1rem', opacity: 0.6, fontWeight: 400 }}>{g.items.length} Watches</span>
          </h2>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(120px, 1fr))', gap: '1rem' }}>
            {g.items.map((w) => {
              const f = filmById(w.filmId);
              const n = watchNumber(watches, w);
              return (
                <button
                  key={w.id}
                  data-watch-id={w.id}
                  type="button"
                  onClick={() => setFilm(f)}
                  title={`${f.title} (${f.year})`}
                  style={{ position: 'relative', background: 'none', border: 'none', padding: 4, color: 'inherit', textAlign: 'center', borderRadius: 6, ...highlightStyle(w.id === highlightId) }}
                >
                  <div style={{ display: 'flex', justifyContent: 'center' }}><Poster film={f} width={110} /></div>
                  {n > 1 && (
                    <span style={{ position: 'absolute', top: 8, right: 8, background: '#f5c542', color: '#222', fontSize: 11, fontWeight: 700, borderRadius: 10, padding: '1px 6px' }}>
                      {ordinal(n)}
                    </span>
                  )}
                  <div style={{ fontSize: '0.8rem', marginTop: 4, opacity: 0.8 }}>{fmt(w.date, { day: 'numeric', month: 'short' })}</div>
                </button>
              );
            })}
          </div>
        </section>
      ))}

      <button
        type="button"
        aria-label="Log a Watch"
        onClick={() => { setSearching(true); setPicked(null); setDate(todayIso()); setError(null); }}
        style={{ position: 'fixed', right: 24, bottom: 80, width: 56, height: 56, borderRadius: 28, border: 'none', fontSize: 28, color: 'white', background: 'linear-gradient(135deg, var(--accent-gradient-start), var(--accent-gradient-end))', boxShadow: '0 4px 12px rgba(0,0,0,.4)' }}
      >
        +
      </button>

      {(film || searching) && (
        <div
          onClick={closeAll}
          style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,.6)', display: 'grid', placeItems: 'center', zIndex: 50 }}
        >
          <div onClick={(e) => e.stopPropagation()} style={{ background: 'var(--card-bg)', borderRadius: 12, padding: '1.5rem', width: 'min(520px, 92vw)' }}>
            {searching && !picked && (
              <>
                <h2 style={{ marginTop: 0 }}>Log a Watch</h2>
                <StubSearch autoFocus onPick={(f) => { setPicked(f); setError(null); }} />
              </>
            )}
            {searching && picked && (
              <div style={{ display: 'flex', gap: '1.25rem' }}>
                <Poster film={picked} width={110} />
                <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '0.6rem' }}>
                  <h2 style={{ margin: 0 }}>{picked.title}</h2>
                  <div style={{ opacity: 0.7 }}>{picked.year}</div>
                  <label style={{ display: 'flex', flexDirection: 'column', gap: 4, fontSize: '0.85rem' }}>
                    Date watched
                    <input
                      type="date"
                      required
                      max={todayIso()}
                      value={date}
                      onChange={(e) => { setDate(e.target.value); setError(null); }}
                      style={{ padding: '0.5rem', borderRadius: 6, border: '1px solid #888', font: 'inherit' }}
                    />
                  </label>
                  {error && <div role="alert" style={{ color: 'var(--error-text)', fontSize: '0.85rem' }}>{error}</div>}
                  <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', marginTop: 'auto' }}>
                    <button type="button" onClick={() => setPicked(null)} style={{ padding: '0.5rem 1rem', borderRadius: 6, border: '1px solid #888', background: 'none', color: 'inherit' }}>Cancel</button>
                    <button type="button" onClick={save} style={{ padding: '0.5rem 1rem', borderRadius: 6, border: 'none', color: 'white', background: 'linear-gradient(135deg, var(--accent-gradient-start), var(--accent-gradient-end))' }}>Save</button>
                  </div>
                </div>
              </div>
            )}
            {film && (
              <div style={{ display: 'flex', gap: '1.25rem' }}>
                <Poster film={film} width={140} />
                <div>
                  <h2 style={{ margin: 0 }}>{film.title}</h2>
                  <div style={{ opacity: 0.7 }}>{film.year} · {film.director}</div>
                  <h3 style={{ fontSize: '0.9rem', marginBottom: 4 }}>Watched {watchesOfFilm(watches, film.id).length}×</h3>
                  <ul style={{ margin: 0, paddingLeft: '1.1rem' }}>
                    {watchesOfFilm(watches, film.id).map((x) => (
                      <li key={x.id}>{fmt(x.date, { weekday: 'short', day: 'numeric', month: 'long', year: 'numeric' })}</li>
                    ))}
                  </ul>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
