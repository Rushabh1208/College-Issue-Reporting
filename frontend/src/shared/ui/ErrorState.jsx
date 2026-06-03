import { AlertTriangle } from "lucide-react";
import { Button } from "./Button";
import { Card } from "./Card";

export function ErrorState({ title = "Unable to load data", message, onRetry }) {
  return (
    <Card className="border-red-100 bg-red-50">
      <div className="flex gap-3">
        <AlertTriangle className="mt-0.5 h-5 w-5 shrink-0 text-red-600" aria-hidden="true" />
        <div>
          <h2 className="font-bold text-red-950">{title}</h2>
          <p className="mt-1 text-sm leading-6 text-red-700">{message}</p>
          {onRetry && (
            <Button className="mt-4" variant="secondary" onClick={onRetry}>
              Retry
            </Button>
          )}
        </div>
      </div>
    </Card>
  );
}
