export const API_BASE =
  process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5290';

export type Incident = {
  id: string;
  title: string;
  description?: string | null;
  severity: 1 | 2 | 3 | 4;
  status: 1 | 2 | 3;
  createdAt: string;
  acknowledgedAt?: string | null;
  resolvedAt?: string | null;
  assignee?: string | null;
};

export async function listIncidents(page = 1, pageSize = 20) {
  const res = await fetch(`${API_BASE}/incidents?page=${page}&pageSize=${pageSize}`, {
    cache: 'no-store',
  });
  if (!res.ok) throw new Error('Failed to load incidents');
  return res.json() as Promise<{ total: number; page: number; pageSize: number; items: Incident[] }>;
}

export async function createIncident(data: {
  title: string;
  description?: string;
  severity: 1 | 2 | 3 | 4;
  assignee?: string;
}) {
  const res = await fetch(`${API_BASE}/incidents`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    const msg = await res.text();
    throw new Error(msg || 'Failed to create incident');
  }
  return res.json() as Promise<{ id: string }>;
}
