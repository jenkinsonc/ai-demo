// PROTOTYPE - throwaway. Mock Films and Watches for the "What the Diary looks like" prototype.
// In-memory only; nothing is persisted.

export interface Film {
  id: number;
  title: string;
  year: number;
  director: string;
  hue: number; // stands in for the TMDB poster
  hasPoster: boolean;
}

export interface Watch {
  id: number;
  filmId: number;
  date: string; // yyyy-mm-dd, local calendar date
}

export const films: Film[] = [
  { id: 1, title: 'The Long Goodbye', year: 1973, director: 'Robert Altman', hue: 30, hasPoster: true },
  { id: 2, title: 'Paris, Texas', year: 1984, director: 'Wim Wenders', hue: 10, hasPoster: true },
  { id: 3, title: 'In the Mood for Love', year: 2000, director: 'Wong Kar-wai', hue: 350, hasPoster: true },
  { id: 4, title: 'Alien', year: 1979, director: 'Ridley Scott', hue: 160, hasPoster: true },
  { id: 5, title: 'Past Lives', year: 2023, director: 'Celine Song', hue: 210, hasPoster: true },
  { id: 6, title: 'The Thing', year: 1982, director: 'John Carpenter', hue: 190, hasPoster: true },
  { id: 7, title: 'Aftersun', year: 2022, director: 'Charlotte Wells', hue: 45, hasPoster: true },
  { id: 8, title: 'Stalker', year: 1979, director: 'Andrei Tarkovsky', hue: 90, hasPoster: true },
  { id: 9, title: 'A Little Seen Festival Short Feature With A Very Long Title Indeed', year: 2025, director: 'Unknown Director', hue: 0, hasPoster: false },
  { id: 10, title: 'Heat', year: 1995, director: 'Michael Mann', hue: 220, hasPoster: true },
  { id: 11, title: 'Spirited Away', year: 2001, director: 'Hayao Miyazaki', hue: 280, hasPoster: true },
  { id: 12, title: 'Jaws', year: 1975, director: 'Steven Spielberg', hue: 200, hasPoster: true },
  { id: 13, title: 'Perfect Days', year: 2023, director: 'Wim Wenders', hue: 120, hasPoster: true },
  { id: 14, title: 'Mulholland Drive', year: 2001, director: 'David Lynch', hue: 260, hasPoster: true },
];

// Rewatches: Alien x3, Paris Texas x2, Heat x2, Spirited Away x2. Two Watches on one day (2026-10-31).
export const initialWatches: Watch[] = [
  { id: 1, filmId: 5, date: '2026-11-20' },
  { id: 2, filmId: 4, date: '2026-11-15' },
  { id: 3, filmId: 7, date: '2026-11-09' },
  { id: 4, filmId: 6, date: '2026-10-31' },
  { id: 5, filmId: 4, date: '2026-10-31' },
  { id: 6, filmId: 9, date: '2026-10-18' },
  { id: 7, filmId: 2, date: '2026-10-04' },
  { id: 8, filmId: 10, date: '2026-09-22' },
  { id: 9, filmId: 13, date: '2026-09-07' },
  { id: 10, filmId: 11, date: '2026-08-30' },
  { id: 11, filmId: 1, date: '2026-08-12' },
  { id: 12, filmId: 3, date: '2026-07-19' },
  { id: 13, filmId: 8, date: '2026-06-02' },
  { id: 14, filmId: 12, date: '2026-01-04' },
  { id: 15, filmId: 14, date: '2025-12-28' },
  { id: 16, filmId: 2, date: '2025-11-11' },
  { id: 17, filmId: 4, date: '2025-10-30' },
  { id: 18, filmId: 10, date: '2025-06-14' },
  { id: 19, filmId: 11, date: '2024-12-24' },
];

export const filmById = (id: number): Film => films.find((f) => f.id === id)!;

export const todayIso = (): string => {
  const d = new Date();
  const m = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${d.getFullYear()}-${m}-${day}`;
};

export const newestFirst = (a: Watch, b: Watch): number =>
  a.date === b.date ? b.id - a.id : b.date.localeCompare(a.date);

export const parseIso = (iso: string): Date => {
  const [y, m, d] = iso.split('-').map(Number);
  return new Date(y, m - 1, d);
};

export const fmt = (iso: string, opts: Intl.DateTimeFormatOptions): string =>
  parseIso(iso).toLocaleDateString(undefined, opts);

/** 1-based position of this Watch among all Watches of its Film, oldest first. */
export const watchNumber = (watches: Watch[], w: Watch): number =>
  watches
    .filter((x) => x.filmId === w.filmId)
    .sort((a, b) => -newestFirst(a, b))
    .findIndex((x) => x.id === w.id) + 1;

export const watchesOfFilm = (watches: Watch[], filmId: number): Watch[] =>
  watches.filter((w) => w.filmId === filmId).sort(newestFirst);

export const highlightStyle = (on: boolean): React.CSSProperties =>
  on ? { outline: '2px solid #f5c542', background: 'rgba(245,197,66,.15)', transition: 'background 1s' } : {};

export const ordinal =(n: number): string => {
  const s = ['th', 'st', 'nd', 'rd'];
  const v = n % 100;
  return n + (s[(v - 20) % 10] || s[v] || s[0]);
};
