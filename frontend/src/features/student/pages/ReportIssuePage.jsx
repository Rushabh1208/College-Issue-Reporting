import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { Send } from "lucide-react";
import { reportIssue, getCategories } from "../api/studentIssueApi";
import { ImageUploadField } from "../components/ImageUploadField";
import { issueFormRules } from "../../issues/schemas/issueSchemas";
import { useUiStore } from "../../../app/store/uiStore";
import { Button } from "../../../shared/ui/Button";
import { Card } from "../../../shared/ui/Card";
import { FormField, inputClass } from "../../../shared/ui/FormField";

export default function ReportIssuePage() {
  const navigate = useNavigate();
  const pushToast = useUiStore((state) => state.pushToast);
  const [progress, setProgress] = useState(0);
  const [categoriesData, setCategoriesData] = useState([]);

  useEffect(() => {
    getCategories().then(setCategoriesData).catch(console.error);
  }, []);

  const {
    register,
    handleSubmit,
    watch,
    resetField,
    formState: { errors, isSubmitting }
  } = useForm({
    mode: "onBlur",
    defaultValues: { 
      title: "", 
      description: "", 
      block: "", 
      roomNumber: "", 
      image: null,
      categoryId: "",
      priority: "Medium",
      isAnonymous: false
    }
  });

  const selectedCategoryId = watch("categoryId");
  const selectedCategory = useMemo(() => {
    if (!selectedCategoryId) return null;
    for (const group of categoriesData) {
      const cat = group.categories.find(c => c.id.toString() === selectedCategoryId.toString());
      if (cat) return cat;
    }
    return null;
  }, [selectedCategoryId, categoriesData]);

  const isWomenWelfare = selectedCategory?.isWomenWelfare;

  async function onSubmit(values) {
    try {
      await reportIssue(values, (event) => {
        if (event.total) setProgress(Math.round((event.loaded * 100) / event.total));
      });
      pushToast({ type: "success", title: "Issue reported", message: "Your report is now visible in your tracker." });
      navigate("/student/issues", { replace: true });
    } catch (error) {
      pushToast({ type: "error", title: "Could not submit issue", message: error.message });
    }
  }

  return (
    <Card className="mx-auto max-w-2xl">
      <div className="mb-5">
        <h2 className="text-xl font-black text-slate-950">Report an issue</h2>
        <p className="mt-1 text-sm leading-6 text-slate-600">Add the location and a short description so staff can act quickly.</p>
      </div>
      <form className="grid gap-4" onSubmit={handleSubmit(onSubmit)}>
        <FormField label="Title" error={errors.title?.message}>
          <input className={inputClass(errors.title)} placeholder="Broken fan, water leak, damaged bench" {...register("title", issueFormRules.title)} />
        </FormField>
        <FormField label="Description" error={errors.description?.message}>
          <textarea className={`${inputClass(errors.description)} min-h-32 resize-y`} placeholder="Describe what is wrong and where to find it." {...register("description", issueFormRules.description)} />
        </FormField>
        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Block" error={errors.block?.message}>
            <input className={inputClass(errors.block)} placeholder="A" {...register("block", issueFormRules.block)} />
          </FormField>
          <FormField label="Room number" error={errors.roomNumber?.message}>
            <input className={inputClass(errors.roomNumber)} placeholder="302" {...register("roomNumber", issueFormRules.roomNumber)} />
          </FormField>
        </div>
        <div className="grid gap-4 sm:grid-cols-2">
          <FormField label="Category" error={errors.categoryId?.message}>
            <select className={inputClass(errors.categoryId)} {...register("categoryId", { required: "Category is required" })}>
              <option value="">Select a category</option>
              {categoriesData.map((group) => (
                <optgroup key={group.parentCategory} label={group.parentCategory}>
                  {group.categories.map((cat) => (
                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                  ))}
                </optgroup>
              ))}
            </select>
          </FormField>
          <FormField label="Priority" error={errors.priority?.message}>
            <select className={inputClass(errors.priority)} {...register("priority")}>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>
          </FormField>
        </div>
        {isWomenWelfare && (
          <div className="rounded-md bg-blue-50 p-4 border border-blue-200">
            <p className="text-sm text-blue-800 font-medium">This report will only be visible to WomenCell.</p>
          </div>
        )}
        <div className="mt-2">
          <label className="flex items-center gap-2 text-sm text-slate-700 font-medium">
            <input type="checkbox" {...register("isAnonymous")} className="h-4 w-4 rounded border-slate-300 text-brand-600 focus:ring-brand-600" />
            Report Anonymously
          </label>
        </div>
        <ImageUploadField register={register("image", issueFormRules.image)} watch={watch} error={errors.image?.message} onClear={() => resetField("image")} />
        {isSubmitting && progress > 0 && (
          <div className="h-2 overflow-hidden rounded-full bg-slate-100">
            <div className="h-full rounded-full bg-brand-600 transition-all" style={{ width: `${progress}%` }} />
          </div>
        )}
        <div className="sticky bottom-20 -mx-4 bg-white/90 px-4 py-3 backdrop-blur sm:static sm:mx-0 sm:bg-transparent sm:px-0 sm:py-0">
          <Button className="w-full" type="submit" isLoading={isSubmitting}>
            <Send className="h-4 w-4" aria-hidden="true" />
            Submit report
          </Button>
        </div>
      </form>
    </Card>
  );
}
