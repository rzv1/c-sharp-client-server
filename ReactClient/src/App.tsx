import { useState, useEffect } from 'react';
import { RaceForm } from './RaceForm';
import { RaceList } from './RaceList';
import { getRaces, createRace, updateRace, deleteRace } from './api';
import type { Race } from './api';

function App() {
  const [races, setRaces] = useState<Race[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editingRace, setEditingRace] = useState<Race | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [filter, setFilter] = useState('');

  useEffect(() => {
    fetchRaces();
  }, []);

  const fetchRaces = async () => {
    try {
      setLoading(true);
      const data = await getRaces();
      setRaces(data);
      setError(null);
    } catch (err) {
      setError('Failed to load races. Make sure the API server is running.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddRace = () => {
    setEditingRace(null);
  };

  const handleEditRace = (race: Race) => {
    setEditingRace(race);
  };

  const handleCancelEdit = () => {
    setEditingRace(null);
  };

  const handleSubmit = async (raceData: Race) => {
    try {
      setIsSubmitting(true);
      if (raceData.id) {
        // Update existing
        const updated = await updateRace(raceData.id, raceData);
        setRaces(races.map(r => r.id === updated.id ? updated : r));
      } else {
        // Create new
        const created = await createRace(raceData);
        setRaces([...races, created]);
      }
      setEditingRace(null);
      setError(null);
    } catch (err) {
      setError('Failed to save race.');
      console.error(err);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this race?')) return;
    
    try {
      setDeletingId(id);
      await deleteRace(id);
      setRaces(races.filter(r => r.id !== id));
      setError(null);
    } catch (err) {
      setError('Failed to delete race.');
      console.error(err);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <>
      <header className="app-header">
        <h1 className="app-title">Swim Contest Manager</h1>
      </header>

      {error && (
        <div style={{ backgroundColor: 'var(--danger-color)', padding: '1rem', borderRadius: '8px', marginBottom: '2rem', textAlign: 'center' }}>
          {error}
        </div>
      )}

      {loading ? (
        <div className="loader"></div>
      ) : (
        <div className="dashboard">
          <RaceList 
            races={races} 
            onEdit={handleEditRace} 
            onDelete={handleDelete}
            deletingId={deletingId}
            filter={filter}
            onFilterChange={setFilter}
          />
          
          <div>
            {(editingRace !== null || editingRace === null) && (
              <RaceForm 
                initialData={editingRace} 
                onSubmit={handleSubmit} 
                onCancel={handleCancelEdit}
                isLoading={isSubmitting}
              />
            )}
          </div>
        </div>
      )}
    </>
  );
}

export default App;
