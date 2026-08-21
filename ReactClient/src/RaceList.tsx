import { RaceItem } from './RaceItem';
import type { Race } from './api';

interface RaceListProps {
  races: Race[];
  onEdit: (race: Race) => void;
  onDelete: (id: number) => void;
  deletingId: number | null;
  filter: string;
  onFilterChange: (filter: string) => void;
}

export function RaceList({ races, onEdit, onDelete, deletingId, filter, onFilterChange }: RaceListProps) {
  const filteredRaces = races.filter(race => 
    race.style.toLowerCase().includes(filter.toLowerCase()) || 
    race.distance.toLowerCase().includes(filter.toLowerCase())
  );

  return (
    <div className="panel list-panel">
      <div className="filter-bar">
        <input 
          type="text" 
          className="search-input" 
          placeholder="Search by distance or style..." 
          value={filter}
          onChange={(e) => onFilterChange(e.target.value)}
        />
        <span style={{color: 'var(--text-secondary)'}}>
          {filteredRaces.length} races found
        </span>
      </div>
      
      <div className="race-list">
        {filteredRaces.length === 0 ? (
          <div className="empty-state">
            <p>No races found matching your criteria.</p>
          </div>
        ) : (
          filteredRaces.map(race => (
            <RaceItem 
              key={race.id} 
              race={race} 
              onEdit={onEdit} 
              onDelete={onDelete} 
              isDeleting={deletingId === race.id}
            />
          ))
        )}
      </div>
    </div>
  );
}
