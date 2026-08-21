import type { Race } from './api';

interface RaceItemProps {
  race: Race;
  onEdit: (race: Race) => void;
  onDelete: (id: number) => void;
  isDeleting: boolean;
}

export function RaceItem({ race, onEdit, onDelete, isDeleting }: RaceItemProps) {
  return (
    <div className="race-item">
      <div className="race-info">
        <h3>{race.distance}</h3>
        <p>Style: {race.style}</p>
        <p>Participants: {race.participantCount || (race.participants?.length || 0)}</p>
      </div>
      <div className="race-actions">
        <button 
          className="btn btn-outline" 
          onClick={() => onEdit(race)}
          disabled={isDeleting}
        >
          Edit
        </button>
        <button 
          className="btn btn-danger" 
          onClick={() => race.id && onDelete(race.id)}
          disabled={isDeleting}
        >
          Delete
        </button>
      </div>
    </div>
  );
}
