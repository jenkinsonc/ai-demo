// PROTOTYPE - throwaway. Variant A "Journal": grouped by month, text only, rewatch icon,
// per-Film Watches expand inline under the row. Search sits inline above the list.
import { useState } from 'react';
import { filmById, fmt, watchNumber, watchesOfFilm, newestFirst, highlightStyle, type Watch } from './mockData';
import { StubSearch } from './shared';
import type { VariantProps } from './types';

export function VariantA({ watches, onLog, highlightId }: VariantProps) {
  const [openFilm, setOpenFilm] = useState<{ watchId: number; filmId: number } | null>(null);
  const sorted = [...watches].sort(newestFirst);
  const groups: { key: string; items: Watch[] }[] = [];
  for (const w of sorted) {
    const key = w.date.slice(0, 7);
    const last = groups[groups.length - 1];
    if (last?.key === key) last.items.push(w); else groups.push({ key, items: [w] });
  }

  return (
    <div style={{ maxWidth: 640, margin: '0 auto', width: '100%' }}>
      <StubSearch onPick={onLog} />
      {groups.map((g) => (
        <section key={g.key} style={{ marginTop: '1.5rem' }}>
          <h2 style={{ fontSize: '0.8rem', textTransform: 'uppercase', letterSpacing: '0.12em', opacity: 0.7, borderBottom: '1px solid var(--divider-color)', paddingBottom: 4, margin: 0 }}>
            {fmt(g.items[0].date, { month: 'long', year: 'numeric' })} <span style={{ opacity: 0.6 }}>· {g.items.length}</span>
          </h2>
          <ol style={{ listStyle: 'none', margin: 0, padding: 0 }}>
            {g.items.map((w) => {
              const film = filmById(w.filmId);
              const n = watchNumber(watches, w);
              const isOpen = openFilm?.watchId === w.id;
              return (
                <li key={w.id} style={{ borderBottom: '1px solid var(--weather-card-border)', ...highlightStyle(w.id === highlightId) }}>
                  <div style={{ display: 'flex', alignItems: 'baseline', gap: '1rem', padding: '0.5rem 0.25rem' }}>
                    <span style={{ width: '2.2rem', textAlign: 'right', fontSize: '1.3rem', fontWeight: 700, fontVariantNumeric: 'tabular-nums' }}>
                      {fmt(w.date, { day: 'numeric' })}
                    </span>
                    <span style={{ width: '2.2rem', fontSize: '0.75rem', opacity: 0.6 }}>{fmt(w.date, { weekday: 'short' })}</span>
                    <button
                      type="button"
                      onClick={() => setOpenFilm(isOpen ? null : { watchId: w.id, filmId: w.filmId })}
                      style={{ flex: 1, background: 'none', border: 'none', color: 'inherit', textAlign: 'left', padding: 0, font: 'inherit' }}
                    >
                      {film.title} <span style={{ opacity: 0.6 }}>{film.year}</span>
                    </button>
                    {n > 1 && <span title={`Rewatch (${n} Watches so far)`} style={{ opacity: 0.8 }}>↻</span>}
                  </div>
                  {isOpen && (
                    <div style={{ padding: '0 0 0.75rem 5.4rem', fontSize: '0.85rem', opacity: 0.85 }}>
                      {film.director} · all Watches of this Film:
                      <ul style={{ margin: '0.25rem 0 0', paddingLeft: '1.1rem' }}>
                        {watchesOfFilm(watches, film.id).map((x) => (
                          <li key={x.id} style={{ fontWeight: x.id === w.id ? 700 : 400 }}>
                            {fmt(x.date, { day: 'numeric', month: 'short', year: 'numeric' })}
                          </li>
                        ))}
                      </ul>
                    </div>
                  )}
                </li>
              );
            })}
          </ol>
        </section>
      ))}
    </div>
  );
}
