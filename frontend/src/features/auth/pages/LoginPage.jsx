import { Link, useLocation, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { LogIn } from "lucide-react";
import { login } from "../api/authApi";
import { loginRules } from "../schemas/authSchemas";
import { useAuthStore } from "../store/authStore";
import { roleHome } from "../utils";
import { useUiStore } from "../../../app/store/uiStore";
import { Button } from "../../../shared/ui/Button";
import { Card } from "../../../shared/ui/Card";
import { FormField, inputClass } from "../../../shared/ui/FormField";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const setToken = useAuthStore((state) => state.setToken);
  const pushToast = useUiStore((state) => state.pushToast);
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm({ defaultValues: { identifier: "", password: "" } });

  async function onSubmit(values) {
    try {
      const response = await login(values);
      const ok = setToken(response.token);
      if (!ok) throw new Error("The login token could not be read.");
      const role = useAuthStore.getState().user.role;
      pushToast({ type: "success", title: "Signed in", message: "Welcome back." });
      navigate(location.state?.from || roleHome(role), { replace: true });
    } catch (error) {
      pushToast({ type: "error", title: "Login failed", message: error.message });
    }
  }

  return (
    <Card>
      <form className="grid gap-4" onSubmit={handleSubmit(onSubmit)}>
        <FormField label="Student ID or Email" error={errors.identifier?.message}>
          <input
            className={inputClass(errors.identifier)}
            autoComplete="username"
            placeholder="e.g. 2025001 or you@college.edu"
            {...register("identifier", loginRules.identifier)}
          />
        </FormField>
        <FormField label="Password" error={errors.password?.message}>
          <input
            className={inputClass(errors.password)}
            type="password"
            autoComplete="current-password"
            {...register("password", loginRules.password)}
          />
        </FormField>
        <Button className="mt-2 w-full" type="submit" isLoading={isSubmitting}>
          <LogIn className="h-4 w-4" aria-hidden="true" />
          Sign in
        </Button>
      </form>
      <div className="mt-5 rounded-lg bg-slate-50 p-3 text-xs leading-5 text-slate-600">
        Staff and admin use their email address. Students use their Student ID or college email.
      </div>
    </Card>
  );
}
