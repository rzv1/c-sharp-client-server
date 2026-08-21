import { useState, useEffect } from 'react';
import type { Race } from './api';

interface RaceFormProps {
  initialData: Race | null;
  onSubmit: (race: Race) => void;
  onCancel: () => void;
  isLoading: boolean;
}

export function RaceForm({ initialData, onSubmit, onCancel, isLoading }: RaceFormProps) {
  const [distance, setDistance] = useState('');
  const [style, setStyle] = useState('');

  useEffect(() => {
    if (initialData) {
      setDistance(initialData.distance);
      setStyle(initialData.style);
    } else {
      setDistance('');
      setStyle('');
    }
  }, [initialData]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!distance.trim() || !style.trim()) return;
    
    onSubmit({
      id: initialData?.id,
      distance,
      style,
      participants: initialData?.participants || []
    });
  };

  return (
    <div className="panel form-panel">
      <h2 className="panel-title">{initialData ? 'Edit Race' : 'Add New Race'}</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label className="form-label" htmlFor="distance">Distance</label>
          <input
            id="distance"
            className="form-input"
            type="text"
            value={distance}
            onChange={(e) => setDistance(e.target.value)}
            placeholder="e.g. 100m, 200m"
            required
            disabled={isLoading}
          />
        </div>
        <div className="form-group">
          <label className="form-label" htmlFor="style">Style</label>
          <input
            id="style"
            className="form-input"
            type="text"
            value={style}
            onChange={(e) => setStyle(e.target.value)}
            placeholder="e.g. freestyle, backstroke"
            required
            disabled={isLoading}
          />
        </div>
        <div className="form-actions">
          <button type="submit" className="btn" style={{ flex: 1 }} disabled={isLoading}>
            {isLoading ? 'Saving...' : (initialData ? 'Update Race' : 'Add Race')}
          </button>
          <button type="button" className="btn btn-outline" onClick={onCancel} disabled={isLoading}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
