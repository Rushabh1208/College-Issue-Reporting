import { Card } from "../../../shared/ui/Card";

export function IssueStats({ issues }) {
  const counts = {
    Open: issues.filter((issue) => issue.status === "Open").length,
    InProgress: issues.filter((issue) => issue.status === "InProgress").length,
    Resolved: issues.filter((issue) => issue.status === "Resolved").length
  };

  return (
    <div className="grid grid-cols-3 gap-2">
      {Object.entries(counts).map(([label, value]) => (
        <Card key={label} className="p-3">
          <p className="text-xl font-black text-slate-950">{value}</p>
          <p className="mt-0.5 text-xs font-semibold text-slate-500">{label === "InProgress" ? "In progress" : label}</p>
        </Card>
      ))}
    </div>
  );
}
