"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { Form, Input, Button, Select } from "antd";
import toast, { Toaster } from "react-hot-toast";

const { TextArea } = Input;
const { Option } = Select;

export default function NewIncidentPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  const onFinish = async (values: any) => {
    try {
      setLoading(true);

      const res = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL}/incidents`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(values),
      });

      if (!res.ok) throw new Error("Failed to create incident");

      toast.success("Incident created successfully!");

      // Redirect to incidents list after a short delay
      setTimeout(() => {
        router.push("/incidents");
      }, 1000);
    } catch (error) {
      toast.error("Failed to create incident");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: 600, margin: "2rem auto" }}>
      <h1 style={{ marginBottom: "1rem" }}>Create New Incident</h1>

      <Form layout="vertical" onFinish={onFinish}>
        {/* Title */}
        <Form.Item
          label="Title"
          name="title"
          rules={[{ required: true, message: "Please enter a title" }]}
        >
          <Input placeholder="Enter incident title" />
        </Form.Item>

        {/* Description */}
        <Form.Item
          label="Description"
          name="description"
          rules={[{ required: true, message: "Please enter a description" }]}
        >
          <TextArea rows={4} placeholder="Describe the incident" />
        </Form.Item>

        <Form.Item
          label="Severity"
          name="severity"
          rules={[{ required: true, message: "Please select severity" }]}
        >
          <Select placeholder="Select severity">
            <Option value={0}>Low</Option>
            <Option value={1}>Medium</Option>
            <Option value={2}>High</Option>
            <Option value={3}>Critical</Option>
          </Select>
        </Form.Item>

        <Form.Item
          label="Status"
          name="status"
          initialValue={0} // Open
          rules={[{ required: true, message: "Please select status" }]}
        >
          <Select placeholder="Select status">
            <Option value={0}>Open</Option>
            <Option value={1}>Acknowledged</Option>
            <Option value={2}>Resolved</Option>
          </Select>
        </Form.Item>

        {/* Assignee */}
        <Form.Item label="Assignee" name="assignee">
          <Input placeholder="Enter assignee name (optional)" />
        </Form.Item>

        {/* Submit */}
        <Form.Item>
          <Button type="primary" htmlType="submit" loading={loading}>
            Create Incident
          </Button>
          <Button
            style={{ marginLeft: "1rem" }}
            onClick={() => router.push("/incidents")}
          >
            Back to Incidents
          </Button>
        </Form.Item>
      </Form>

      <Toaster position="top-right" />
    </div>
  );
}
