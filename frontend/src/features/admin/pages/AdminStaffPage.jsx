import { useState, useEffect } from "react";
import { PlusCircle, Edit2, Ban, KeyRound, CheckCircle2, XCircle } from "lucide-react";
import { getStaff, createStaff, updateStaff, deactivateStaff, resetStaffPassword } from "../../users/api/staffApi";
import { Button } from "../../../shared/ui/Button";
import { Card } from "../../../shared/ui/Card";
import { Pagination } from "../../../shared/ui/Pagination";
import { useUiStore } from "../../../app/store/uiStore";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { SkeletonList } from "../../../shared/ui/Skeleton";

export default function AdminStaffPage() {
  const [staffList, setStaffList] = useState([]);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingStaff, setEditingStaff] = useState(null);
  const [formData, setFormData] = useState({ name: "", email: "" });
  const [isSaving, setIsSaving] = useState(false);
  
  const pushToast = useUiStore((state) => state.pushToast);

  const fetchStaff = async (p = 1) => {
    setIsLoading(true);
    try {
      const res = await getStaff({ page: p, pageSize: 20 });
      setStaffList(res.items);
      setTotal(res.total);
      setPage(res.page);
    } catch (error) {
      pushToast({ type: "error", title: "Error", message: "Failed to load staff list" });
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchStaff(page);
  }, [page]);

  const handleOpenModal = (staff = null) => {
    if (staff) {
      setEditingStaff(staff);
      setFormData({ name: staff.name, email: staff.email });
    } else {
      setEditingStaff(null);
      setFormData({ name: "", email: "" });
    }
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingStaff(null);
    setFormData({ name: "", email: "" });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSaving(true);
    try {
      if (editingStaff) {
        await updateStaff(editingStaff.id, formData);
        pushToast({ type: "success", title: "Success", message: "Staff account updated successfully" });
      } else {
        await createStaff(formData);
        pushToast({ type: "success", title: "Success", message: "Staff account created successfully" });
      }
      handleCloseModal();
      fetchStaff(page);
    } catch (error) {
      const msg = error.response?.data?.message || error.response?.data?.[0]?.errorMessage || "Failed to save staff account";
      pushToast({ type: "error", title: "Error", message: msg });
    } finally {
      setIsSaving(false);
    }
  };

  const handleDeactivate = async (id) => {
    if (!window.confirm("Are you sure you want to deactivate this staff account? They will not be able to log in.")) return;
    try {
      await deactivateStaff(id);
      pushToast({ type: "success", title: "Deactivated", message: "Staff deactivated successfully." });
      fetchStaff(page);
    } catch (error) {
      pushToast({ type: "error", title: "Error", message: "Failed to deactivate staff." });
    }
  };

  const handleResetPassword = async (id) => {
    if (!window.confirm("Are you sure you want to reset this staff's password to the default (Staff@123)?")) return;
    try {
      await resetStaffPassword(id);
      pushToast({ type: "success", title: "Password Reset", message: "Password reset to default successfully." });
    } catch (error) {
      pushToast({ type: "error", title: "Error", message: "Failed to reset password." });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-black text-slate-950">Staff Management</h2>
          <p className="text-sm text-slate-600 mt-1">Manage operational staff accounts and access.</p>
        </div>
        <Button onClick={() => handleOpenModal()}>
          <PlusCircle className="w-4 h-4 mr-2" />
          Add Staff
        </Button>
      </div>

      <Card className="p-0 overflow-hidden">
        {isLoading ? (
          <div className="p-6">
            <SkeletonList rows={5} />
          </div>
        ) : staffList.length === 0 ? (
          <div className="p-10">
            <EmptyState title="No staff found" description="Click 'Add Staff' to create a new staff account." />
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm whitespace-nowrap">
              <thead className="bg-slate-50 text-slate-500">
                <tr>
                  <th className="px-4 py-3 font-medium">Name</th>
                  <th className="px-4 py-3 font-medium">Email</th>
                  <th className="px-4 py-3 font-medium">Status</th>
                  <th className="px-4 py-3 font-medium">Created Date</th>
                  <th className="px-4 py-3 font-medium text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200">
                {staffList.map((s) => (
                  <tr key={s.id} className="hover:bg-slate-50">
                    <td className="px-4 py-3 font-medium text-slate-900">{s.name}</td>
                    <td className="px-4 py-3 text-slate-600">{s.email}</td>
                    <td className="px-4 py-3">
                      {s.isActive ? (
                        <span className="inline-flex items-center gap-1.5 rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800">
                          <CheckCircle2 className="h-3 w-3" /> Active
                        </span>
                      ) : (
                        <span className="inline-flex items-center gap-1.5 rounded-full bg-slate-100 px-2 py-0.5 text-xs font-medium text-slate-800">
                          <XCircle className="h-3 w-3" /> Inactive
                        </span>
                      )}
                    </td>
                    <td className="px-4 py-3 text-slate-500">
                      {new Date(s.createdAt).toLocaleDateString()}
                    </td>
                    <td className="px-4 py-3 text-right">
                      <div className="flex items-center justify-end gap-2">
                        <button
                          onClick={() => handleOpenModal(s)}
                          title="Edit Staff"
                          className="p-1.5 text-slate-400 hover:text-brand-600 hover:bg-brand-50 rounded"
                        >
                          <Edit2 className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleResetPassword(s.id)}
                          title="Reset Password"
                          className="p-1.5 text-slate-400 hover:text-amber-600 hover:bg-amber-50 rounded"
                        >
                          <KeyRound className="w-4 h-4" />
                        </button>
                        {s.isActive && (
                          <button
                            onClick={() => handleDeactivate(s.id)}
                            title="Deactivate"
                            className="p-1.5 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded"
                          >
                            <Ban className="w-4 h-4" />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        
        {total > 0 && (
          <div className="p-4 border-t border-slate-200 bg-slate-50">
            <Pagination
              currentPage={page}
              totalPages={Math.ceil(total / 20)}
              onPageChange={setPage}
            />
          </div>
        )}
      </Card>

      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm px-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-md p-6">
            <h3 className="text-xl font-bold text-slate-900 mb-4">
              {editingStaff ? "Edit Staff" : "Add Staff"}
            </h3>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Name</label>
                <input
                  type="text"
                  required
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none"
                  placeholder="John Doe"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">Email</label>
                <input
                  type="email"
                  required
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-brand-500 focus:border-brand-500 outline-none"
                  placeholder="john@example.com"
                />
              </div>
              {!editingStaff && (
                <p className="text-xs text-slate-500">
                  Default password will be set to <span className="font-mono font-bold">Staff@123</span>
                </p>
              )}
              <div className="flex items-center justify-end gap-3 mt-6">
                <Button type="button" variant="secondary" onClick={handleCloseModal}>
                  Cancel
                </Button>
                <Button type="submit" isLoading={isSaving}>
                  {editingStaff ? "Save Changes" : "Create Staff"}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
