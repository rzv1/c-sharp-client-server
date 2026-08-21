export interface Race {
  id?: number;
  distance: string;
  style: string;
  participants?: number[];
  participantCount?: number;
}

const API_URL = "/api/races";

export const getRaces = async (): Promise<Race[]> => {
  const response = await fetch(API_URL);
  if (!response.ok) throw new Error("Failed to fetch races");
  return response.json();
};

export const getRace = async (id: number): Promise<Race> => {
  const response = await fetch(`${API_URL}/${id}`);
  if (!response.ok) throw new Error("Failed to fetch race");
  return response.json();
};

export const createRace = async (race: Race): Promise<Race> => {
  const response = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(race),
  });
  if (!response.ok) throw new Error("Failed to create race");
  return response.json();
};

export const updateRace = async (id: number, race: Race): Promise<Race> => {
  const response = await fetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(race),
  });
  if (!response.ok) throw new Error("Failed to update race");
  return response.json();
};

export const deleteRace = async (id: number): Promise<void> => {
  const response = await fetch(`${API_URL}/${id}`, {
    method: "DELETE",
  });
  if (!response.ok) throw new Error("Failed to delete race");
};
