import { useForm } from "react-hook-form";
import { KeyRound } from "lucide-react";
import { changePassword } from "../api/accountApi";
import { useUiStore } from "../../../app/store/uiStore";
import { Button } from "../../../shared/ui/Button";
import { Card } from "../../../shared/ui/Card";
import { FormField, inputClass } from "../../../shared/ui/FormField";

export default function ChangePasswordPage() {
  const pushToast = useUiStore((state) => state.pushToast);
  const { register, handleSubmit, watch, reset, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { currentPassword: "", newPassword: "", confirmNewPassword: "" }
  });

  async function onSubmit(values) {
    try {
      await changePassword(values);
      pushToast({ type: "success", title: "Password updated", message: "Your password has been changed." });
      reset();
    } catch (error) {
      pushToast({ type: "error", title: "Update failed", message: error.message });
    }
  }

  return (
    <Card className="mx-auto max-w-md">
      <div className="mb-5">
        <h2 className="text-xl font-black text-slate-950">Change password</h2>
        <p className="mt-1 text-sm text-slate-600">Update your account password.</p>
      </div>
      <form className="grid gap-4" onSubmit={handleSubmit(onSubmit)}>
        <FormField label="Current password" error={errors.currentPassword?.message}>
          <input className={inputClass(errors.currentPassword)} type="password" autoComplete="current-password"
            {...register("currentPassword", { required: "Current password is required" })} />
        </FormField>
        <FormField label="New password" error={errors.newPassword?.message}>
          <input className={inputClass(errors.newPassword)} type="password" autoComplete="new-password"
            {...register("newPassword", { required: "New password is required", minLength: { value: 8, message: "Must be at least 8 characters" } })} />
        </FormField>
        <FormField label="Confirm new password" error={errors.confirmNewPassword?.message}>
          <input className={inputClass(errors.confirmNewPassword)} type="password" autoComplete="new-password"
            {...register("confirmNewPassword", {
              required: "Please confirm your new password",
              validate: (value) => value === watch("newPassword") || "Passwords do not match"
            })} />
        </FormField>
        <Button className="mt-2 w-full" type="submit" isLoading={isSubmitting}>
          <KeyRound className="h-4 w-4" aria-hidden="true" />
          Update password
        </Button>
      </form>
    </Card>
  );
}
