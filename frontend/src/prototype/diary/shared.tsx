// PROTOTYPE - throwaway. Small pieces every Diary variant may use: a fake poster and a stub Film search.
import { useState } from 'react';
import { films, type Film } from './mockData';

export function Poster({ film, width }: { film: Film; width: number }) {
  const height = Math.round(width * 1.5);
  if (!film.hasPoster) {
    return (
      <div
        aria-hidden="true"
        style={{
          width, height, flex: 'none', borderRadius: 4, background: '#555',
          display: 'grid', placeItems: 'center', color: '#bbb', fontSize: width / 4,
        }}
      >
        ?
      </div>
    );
  }
  return (
    <div
      aria-hidden="true"
      style={{
        width, height, flex: 'none', borderRadius: 4,
        background: `linear-gradient(160deg, hsl(${film.hue} 55% 45%), hsl(${film.hue + 40} 50% 18%))`,
        color: 'rgba(255,255,255,.85)', fontSize: Math.max(8, width / 7), fontWeight: 700,
        padding: width / 12, overflow: 'hidden', lineHeight: 1.1, textAlign: 'left',
      }}
    >
      {width >= 40 ? film.title : ''}
    </div>
  );
}

/** Stub search: filters the mock Films; picking one logs a Watch dated today. */
export function StubSearch({ onPick, autoFocus }: { onPick: (film: Film) => void; autoFocus?: boolean }) {
  const [q, setQ] = useState('');
  const hits = q.length >= 2 ? films.filter((f) => f.title.toLowerCase().includes(q.toLowerCase())) : [];
  return (
    <div style={{ position: 'relative' }}>
      <input
        autoFocus={autoFocus}
        value={q}
        onChange={(e) => setQ(e.target.value)}
        placeholder="Search Films to log a Watch (stub: saves as today)"
        style={{ width: '100%', padding: '0.6rem 0.8rem', borderRadius: 6, border: '1px solid #888', font: 'inherit' }}
      />
      {q.length >= 2 && (
        <ul
          style={{
            position: 'absolute', zIndex: 10, left: 0, right: 0, margin: 0, padding: 0, listStyle: 'none',
            background: 'var(--card-bg)', border: '1px solid #888', borderRadius: 6, maxHeight: 260, overflow: 'auto',
          }}
        >
          {hits.length === 0 && <li style={{ padding: '0.5rem 0.8rem' }}>No Films found</li>}
          {hits.map((f) => (
            <li key={f.id}>
              <button
                type="button"
                onClick={() => { onPick(f); setQ(''); }}
                style={{
                  display: 'flex', gap: 8, alignItems: 'center', width: '100%', padding: '0.4rem 0.8rem',
                  background: 'none', border: 'none', color: 'inherit', textAlign: 'left',
                }}
              >
                <Poster film={f} width={24} /> {f.title} ({f.year})
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
