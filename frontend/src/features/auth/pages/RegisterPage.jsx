import { Link, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { UserPlus } from "lucide-react";
import { registerStudent } from "../api/authApi";
import { registerRules } from "../schemas/authSchemas";
import { useUiStore } from "../../../app/store/uiStore";
import { Button } from "../../../shared/ui/Button";
import { Card } from "../../../shared/ui/Card";
import { FormField, inputClass } from "../../../shared/ui/FormField";

export default function RegisterPage() {
  const navigate = useNavigate();
  const pushToast = useUiStore((state) => state.pushToast);
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm({ defaultValues: { name: "", email: "", password: "" }, mode: "onBlur" });

  async function onSubmit(values) {
    try {
      await registerStudent(values);
      pushToast({ type: "success", title: "Account created", message: "Sign in with your new student account." });
      navigate("/login", { replace: true });
    } catch (error) {
      pushToast({ type: "error", title: "Registration failed", message: error.message });
    }
  }

  return (
    <Card>
      <form className="grid gap-4" onSubmit={handleSubmit(onSubmit)}>
        <FormField label="Full name" error={errors.name?.message}>
          <input className={inputClass(errors.name)} autoComplete="name" {...register("name", registerRules.name)} />
        </FormField>
        <FormField label="Email" error={errors.email?.message}>
          <input className={inputClass(errors.email)} autoComplete="email" inputMode="email" {...register("email", registerRules.email)} />
        </FormField>
        <FormField label="Password" error={errors.password?.message}>
          <input className={inputClass(errors.password)} type="password" autoComplete="new-password" {...register("password", registerRules.password)} />
        </FormField>
        <Button className="mt-2 w-full" type="submit" isLoading={isSubmitting}>
          <UserPlus className="h-4 w-4" aria-hidden="true" />
          Create student account
        </Button>
      </form>
      <p className="mt-5 text-center text-sm text-slate-600">
        Already registered? <Link className="font-bold text-brand-600 hover:text-brand-700" to="/login">Sign in</Link>
      </p>
    </Card>
  );
}
