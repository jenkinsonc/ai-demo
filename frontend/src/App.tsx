import aspireLogo from '/Aspire.png';
import './App.css';
// PROTOTYPE - throwaway branch prototype/diary: the weather demo is replaced by the Diary variants.
import { DiaryPrototype } from './prototype/diary/DiaryPrototype';

function App() {
  return (
    <div className="app-container">
      <header className="app-header" style={{ padding: '1rem 2rem' }}>
        <img src={aspireLogo} className="logo" alt="" style={{ height: '2.5rem' }} />
        <h1 className="app-title" style={{ fontSize: '1.75rem', margin: '0.25rem 0 0' }}>Diary</h1>
      </header>

      <main className="main-content" style={{ alignItems: 'flex-start' }}>
        <DiaryPrototype />
      </main>

      <footer className="app-footer">
        <nav aria-label="Footer navigation">
          <span style={{ fontSize: '0.75rem', opacity: 0.7 }}>
            [TMDB logo] This product uses the TMDB API but is not endorsed or certified by TMDB.
          </span>
        </nav>
      </footer>
    </div>
  );
}

export default App;
