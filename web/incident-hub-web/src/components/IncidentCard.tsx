"use client";

import React from "react";
import { Card, Tag, Typography } from "antd";
import { ExclamationCircleOutlined, UserOutlined, ClockCircleOutlined } from "@ant-design/icons";

const { Paragraph, Text } = Typography;

export type Incident = {
  id: string;
  title: string;
  description?: string;
  severity: "Low" | "Medium" | "High";
  status: "Reported" | "Acknowledged" | "Resolved";
  createdAt: string;
  acknowledgedAt?: string;
  resolvedAt?: string;
  assignee?: string;
};

const severityColors: Record<Incident["severity"], string> = {
  Low: "green",
  Medium: "gold",
  High: "red",
};

const statusColors: Record<Incident["status"], string> = {
  Reported: "blue",
  Acknowledged: "purple",
  Resolved: "green",
};

export default function IncidentCard({ incident }: { incident: Incident }) {
  return (
    <Card style={{ marginBottom: 16 }} variant="outlined">
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <Text strong>{incident.title}</Text>
        <div style={{ display: "flex", gap: 8 }}>
          <Tag color={severityColors[incident.severity]}>
            Severity: {incident.severity}
          </Tag>
          <Tag color={statusColors[incident.status]}>
            Status: {incident.status}
          </Tag>
        </div>
      </div>

      <Paragraph type="secondary" style={{ marginTop: 8 }}>
        {incident.description}
      </Paragraph>

      <div style={{ marginTop: 12, fontSize: 13, color: "#555" }}>
        <div>
          <UserOutlined />{" "}
          <Text>
            Assigned to: {incident.assignee ? incident.assignee : "Unassigned"}
          </Text>
        </div>
        <div>
          <ClockCircleOutlined /> Reported on:{" "}
          {new Date(incident.createdAt).toLocaleDateString()}
        </div>
        {incident.acknowledgedAt && (
          <div>
            <ClockCircleOutlined /> Acknowledged at:{" "}
            {new Date(incident.acknowledgedAt).toLocaleDateString()}
          </div>
        )}
        {incident.resolvedAt && (
          <div>
            <ClockCircleOutlined /> Resolved at:{" "}
            {new Date(incident.resolvedAt).toLocaleDateString()}
          </div>
        )}
      </div>
    </Card>
  );
}
