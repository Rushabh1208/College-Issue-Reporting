import React, { useEffect, useState } from "react";
import { getIssueTimeline } from "../../features/issues/api/issuesApi";
import {
  PlusCircle,
  UserCheck,
  RefreshCcw,
  Clock,
  Flag,
  CheckCircle,
  Lock,
  MessageSquare
} from "lucide-react";
import { formatDateTime } from "../utils/formatters";

const iconMap = {
  Created: <PlusCircle size={20} className="text-blue-500" />,
  Assigned: <UserCheck size={20} className="text-purple-500" />,
  Reassigned: <RefreshCcw size={20} className="text-orange-500" />,
  StatusChanged: <Clock size={20} className="text-yellow-500" />,
  PriorityChanged: <Flag size={20} className="text-red-500" />,
  Resolved: <CheckCircle size={20} className="text-green-500" />,
  Closed: <Lock size={20} className="text-gray-500" />,
  Upvoted: <PlusCircle size={20} className="text-pink-500" />,
};

export function IssueTimeline({ issueId }) {
  const [timeline, setTimeline] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchTimeline() {
      try {
        setLoading(true);
        const data = await getIssueTimeline(issueId);
        setTimeline(data);
      } catch (err) {
        setError("Failed to load timeline");
      } finally {
        setLoading(false);
      }
    }
    if (issueId) {
      fetchTimeline();
    }
  }, [issueId]);

  if (loading) return <div className="text-sm text-gray-500 py-4">Loading timeline...</div>;
  if (error) return <div className="text-sm text-red-500 py-4">{error}</div>;
  if (!timeline.length) return <div className="text-sm text-gray-500 py-4">No timeline events found.</div>;

  return (
    <div className="relative pl-4 border-l-2 border-gray-200 ml-3 mt-4 space-y-6">
      {timeline.map((event, index) => (
        <div key={index} className="relative">
          <div className="absolute -left-7 bg-white p-1 rounded-full border border-gray-200">
            {iconMap[event.action] || <MessageSquare size={20} className="text-gray-400" />}
          </div>
          <div className="ml-4">
            <h4 className="text-sm font-semibold text-gray-900">{event.action}</h4>
            <div className="mt-1 text-sm text-gray-600">
              <span className="font-medium text-gray-900">By:</span> {event.performedBy}
            </div>
            {event.notes && (
              <div className="mt-1 text-sm text-gray-700 italic">
                "{event.notes}"
              </div>
            )}
            <div className="mt-1 text-xs text-gray-400">
              {formatDateTime(event.createdAt)}
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}
