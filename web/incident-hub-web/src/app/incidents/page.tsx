'use client';

import { useEffect, useState } from 'react';
import { createIncident, listIncidents, type Incident } from '@/lib/api';

export default function IncidentsPage() {
  const [items, setItems] = useState<Incident[]>([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState<string | null>(null);

  const [title, setTitle] = useState('');
  const [desc, setDesc] = useState('');
  const [severity, setSeverity] = useState<1 | 2 | 3 | 4>(2);
  const [assignee, setAssignee] = useState('');

  async function load() {
    setLoading(true);
    setErr(null);
    try {
      const data = await listIncidents(1, 50);
      setItems(data.items);
    } catch (e) {
      setErr(e instanceof Error ? e.message : 'Error');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { void load(); }, []);

  async function onCreate(e: React.FormEvent) {
    e.preventDefault();
    if (!title.trim()) return;

    try {
      await createIncident({
        title: title.trim(),
        description: desc.trim() || undefined,
        severity,
        assignee: assignee.trim() || undefined,
      });
      setTitle(''); setDesc(''); setAssignee(''); setSeverity(2);
      await load();
    } catch (e) {
      alert(e instanceof Error ? e.message : 'Create failed');
    }
  }

  return (
    <main className="max-w-3xl mx-auto p-6">
      <h1 className="text-3xl font-bold">Incidents</h1>

      <form onSubmit={onCreate} className="mt-6 grid gap-3 border p-4 rounded">
        <input
          className="border p-2 rounded"
          placeholder="Title *"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
        <textarea
          className="border p-2 rounded"
          placeholder="Description"
          value={desc}
          onChange={(e) => setDesc(e.target.value)}
        />
        <div className="flex gap-3">
          <label className="flex items-center gap-2">
            Severity
            <select
              className="border p-2 rounded"
              value={severity}
              onChange={(e) => setSeverity(Number(e.target.value) as 1|2|3|4)}
            >
              <option value={1}>Low</option>
              <option value={2}>Medium</option>
              <option value={3}>High</option>
              <option value={4}>Critical</option>
            </select>
          </label>
          <input
            className="border p-2 rounded flex-1"
            placeholder="Assignee (optional)"
            value={assignee}
            onChange={(e) => setAssignee(e.target.value)}
          />
        </div>
        <button className="bg-blue-600 text-white px-4 py-2 rounded self-start">Create Incident</button>
      </form>

      <section className="mt-8">
        {loading && <p>Loading…</p>}
        {err && <p className="text-red-600">{err}</p>}
        {!loading && !err && (
          <ul className="grid gap-3">
            {items.map(i => (
              <li key={i.id} className="border p-4 rounded">
                <div className="font-semibold">{i.title}</div>
                <div className="text-sm text-gray-600">
                  Severity: {['','Low','Medium','High','Critical'][i.severity]} • Status: {['','Open','Acknowledged','Resolved'][i.status]}
                </div>
                {i.description && <p className="mt-1 text-sm">{i.description}</p>}
                <div className="text-xs text-gray-500 mt-1">
                  Assignee: {i.assignee || '—'} • Created: {new Date(i.createdAt).toLocaleString()}
                </div>
              </li>
            ))}
          </ul>
        )}
      </section>
    </main>
  );
}
