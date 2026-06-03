import { ImagePlus, X } from "lucide-react";
import { useEffect, useMemo } from "react";
import { FormField } from "../../../shared/ui/FormField";

export function ImageUploadField({ register, watch, error, onClear }) {
  const file = watch("image")?.[0];
  const preview = useMemo(() => (file ? URL.createObjectURL(file) : null), [file]);

  useEffect(() => {
    return () => {
      if (preview) URL.revokeObjectURL(preview);
    };
  }, [preview]);

  return (
    <FormField label="Image" error={error} hint="JPEG or PNG, max 5 MB. Camera upload works on mobile.">
      <div className="rounded-lg border border-dashed border-slate-300 bg-white p-3">
        {preview ? (
          <div className="relative overflow-hidden rounded-lg">
            <img src={preview} alt="Selected issue" className="h-52 w-full object-cover" />
            <button
              type="button"
              onClick={onClear}
              className="absolute right-2 top-2 grid h-9 w-9 place-items-center rounded-full bg-white/90 text-slate-700 shadow-soft"
              aria-label="Remove selected image"
            >
              <X className="h-4 w-4" />
            </button>
          </div>
        ) : (
          <div className="grid place-items-center px-4 py-8 text-center">
            <ImagePlus className="h-8 w-8 text-slate-400" aria-hidden="true" />
            <p className="mt-2 text-sm font-semibold text-slate-700">Add a photo</p>
            <p className="mt-1 text-xs text-slate-500">Tap to use camera or gallery.</p>
          </div>
        )}
        <input
          className="mt-3 block w-full cursor-pointer rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-600 file:mr-3 file:rounded-md file:border-0 file:bg-brand-600 file:px-3 file:py-1.5 file:text-sm file:font-bold file:text-white"
          type="file"
          accept="image/jpeg,image/jpg,image/png"
          capture="environment"
          {...register}
        />
      </div>
    </FormField>
  );
}
