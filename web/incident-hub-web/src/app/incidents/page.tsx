"use client";

import React, { useEffect, useState } from "react";
import { Button, Spin, Typography } from "antd";
import Link from "next/link";
import IncidentCard, { Incident } from "@/components/IncidentCard";
import toast, { Toaster } from "react-hot-toast";

const { Title } = Typography;

export default function IncidentsPage() {
  const [incidents, setIncidents] = useState<Incident[]>([]);
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    async function fetchIncidents() {
      try {
        const res = await fetch(
          `${process.env.NEXT_PUBLIC_API_BASE_URL}/incidents`
        );
        if (!res.ok) throw new Error("Failed to fetch incidents");

        const data = await res.json();

        console.log("logging data: ", data)

        // Ensure we always set an array
        if (Array.isArray(data)) {
          setIncidents(data);
        } else if (data && data.items) {
          // if API wraps it like { incidents: [...] }
          setIncidents(data.items);
        } else {
          setIncidents([]); // fallback
        }
      } catch (error) {
        console.error(error);
        toast.error("Could not load incidents");
        setIncidents([]);
      } finally {
        setLoading(false);
      }
    }

    fetchIncidents();
  }, []);

  return (
        <div style={{ padding: 24 }}>
      <Title level={2}>Incidents</Title>

      <div style={{ marginBottom: 16 }}>
        <Link href="/incidents/new">
          <Button type="primary">Create New Incident</Button>
        </Link>{" "}
        <Link href="/">
          <Button>Back to Home</Button>
        </Link>
      </div>

      {loading ? (
        <Spin />
      ) : incidents.length === 0 ? (
        <p>No incidents yet.</p>
      ) : (
        incidents.map((incident) => (
          <IncidentCard key={incident.id} incident={incident} />
        ))
      )}
    </div>
  );
}
