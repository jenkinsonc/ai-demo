// PROTOTYPE - throwaway. Floating bottom bar that cycles ?variant= values. Hidden in production builds.
import { useEffect } from 'react';

interface Props {
  variants: { key: string; name: string }[];
  current: string;
  onChange: (key: string) => void;
}

export function PrototypeSwitcher({ variants, current, onChange }: Props) {
  const i = Math.max(0, variants.findIndex((v) => v.key === current));
  const go = (step: number) => onChange(variants[(i + step + variants.length) % variants.length].key);

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      const t = e.target as HTMLElement | null;
      if (t && (t.tagName === 'INPUT' || t.tagName === 'TEXTAREA' || t.isContentEditable)) return;
      if (e.key === 'ArrowLeft') go(-1);
      if (e.key === 'ArrowRight') go(1);
    };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  });

  if (import.meta.env.PROD) return null;
  const btn: React.CSSProperties = { background: 'none', border: 'none', color: '#fff', fontSize: 18, padding: '0 0.5rem' };
  return (
    <div
      style={{
        position: 'fixed', bottom: 16, left: '50%', transform: 'translateX(-50%)', zIndex: 100,
        display: 'flex', alignItems: 'center', gap: 8, padding: '0.4rem 0.8rem', borderRadius: 999,
        background: '#000', color: '#fff', boxShadow: '0 4px 16px rgba(0,0,0,.5)', fontSize: 14, fontFamily: 'monospace',
      }}
    >
      <button type="button" style={btn} onClick={() => go(-1)} aria-label="Previous variant">←</button>
      <span>{variants[i].key} ({variants[i].name})</span>
      <button type="button" style={btn} onClick={() => go(1)} aria-label="Next variant">→</button>
    </div>
  );
}
