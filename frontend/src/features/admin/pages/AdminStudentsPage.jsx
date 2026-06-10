import { useState, useEffect, useRef } from "react";
import { Upload, XCircle, CheckCircle2, AlertCircle, KeyRound, Ban } from "lucide-react";
import { importStudents, getStudents, deactivateStudent, resetPassword } from "../../students/api/studentApi";
import { Button } from "../../../shared/ui/Button";
import { Card } from "../../../shared/ui/Card";
import { Pagination } from "../../../shared/ui/Pagination";
import { useUiStore } from "../../../app/store/uiStore";
import { EmptyState } from "../../../shared/ui/EmptyState";
import { SkeletonList } from "../../../shared/ui/Skeleton";

export default function AdminStudentsPage() {
  const [file, setFile] = useState(null);
  const [isUploading, setIsUploading] = useState(false);
  const [importResult, setImportResult] = useState(null);
  const [showErrors, setShowErrors] = useState(false);
  const fileInputRef = useRef(null);

  const [students, setStudents] = useState([]);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  
  const pushToast = useUiStore((state) => state.pushToast);

  const fetchStudents = async (p = 1) => {
    setIsLoading(true);
    try {
      const res = await getStudents(p, 20);
      setStudents(res.items);
      setTotal(res.total);
      setPage(res.page);
    } catch (error) {
      pushToast({ type: "error", title: "Error", message: "Failed to load students" });
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchStudents(page);
  }, [page]);

  const handleFileChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      setFile(e.target.files[0]);
      setImportResult(null);
    }
  };

  const handleUpload = async () => {
    if (!file) return;
    setIsUploading(true);
    try {
      const res = await importStudents(file);
      setImportResult(res);
      pushToast({ type: "success", title: "Import Complete", message: `Imported ${res.importedRows} students.` });
      setFile(null);
      if (fileInputRef.current) fileInputRef.current.value = "";
      fetchStudents(1);
    } catch (error) {
      pushToast({ type: "error", title: "Import Failed", message: error.message });
    } finally {
      setIsUploading(false);
    }
  };

  const handleDeactivate = async (id) => {
    if (!window.confirm("Are you sure you want to deactivate this student? They will not be able to log in.")) return;
    try {
      await deactivateStudent(id);
      pushToast({ type: "success", title: "Deactivated", message: "Student deactivated successfully." });
      fetchStudents(page);
    } catch (error) {
      pushToast({ type: "error", title: "Error", message: "Failed to deactivate student." });
    }
  };

  const handleResetPassword = async (id) => {
    if (!window.confirm("Are you sure you want to reset this student's password to the default?")) return;
    try {
      await resetPassword(id);
      pushToast({ type: "success", title: "Password Reset", message: "Password reset to default successfully." });
    } catch (error) {
      pushToast({ type: "error", title: "Error", message: "Failed to reset password." });
    }
  };

  return (
    <div className="space-y-6">
      <Card className="p-6">
        <h2 className="text-lg font-bold text-slate-900 mb-4">Import Students</h2>
        <div className="flex items-end gap-4">
          <div className="flex-1">
            <label className="block text-sm font-medium text-slate-700 mb-1">Upload CSV File</label>
            <input 
              type="file" 
              accept=".csv" 
              onChange={handleFileChange}
              ref={fileInputRef}
              className="block w-full text-sm text-slate-500 file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-brand-50 file:text-brand-700 hover:file:bg-brand-100"
            />
          </div>
          <Button onClick={handleUpload} disabled={!file || isUploading} isLoading={isUploading}>
            <Upload className="w-4 h-4 mr-2" />
            Import
          </Button>
        </div>

        {importResult && (
          <div className="mt-6 border rounded-lg p-4 bg-slate-50">
            <h3 className="font-semibold text-slate-800 mb-3">Import Summary</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-4">
              <div className="bg-white p-3 rounded shadow-sm border">
                <p className="text-xs text-slate-500">Total Rows</p>
                <p className="font-bold text-lg">{importResult.totalRows}</p>
              </div>
              <div className="bg-white p-3 rounded shadow-sm border">
                <p className="text-xs text-slate-500">Imported</p>
                <p className="font-bold text-lg text-emerald-600">{importResult.importedRows}</p>
              </div>
              <div className="bg-white p-3 rounded shadow-sm border">
                <p className="text-xs text-slate-500">Skipped</p>
                <p className="font-bold text-lg text-amber-600">{importResult.skippedRows}</p>
              </div>
              <div className="bg-white p-3 rounded shadow-sm border">
                <p className="text-xs text-slate-500">Duplicates</p>
                <p className="font-bold text-lg text-slate-600">{importResult.duplicateRows}</p>
              </div>
            </div>

            {importResult.errors?.length > 0 && (
              <div>
                <button 
                  onClick={() => setShowErrors(!showErrors)}
                  className="text-sm font-medium text-brand-600 hover:text-brand-800 flex items-center gap-1"
                >
                  <AlertCircle className="w-4 h-4" />
                  {showErrors ? "Hide Validation Errors" : `Show Validation Errors (${importResult.errors.length})`}
                </button>
                {showErrors && (
                  <div className="mt-3 max-h-40 overflow-y-auto bg-white border rounded p-2 text-sm text-red-600">
                    <ul className="list-disc pl-5 space-y-1">
                      {importResult.errors.map((err, idx) => (
                        <li key={idx}>Row {err.rowNumber}: {err.message}</li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            )}
          </div>
        )}
      </Card>

      <Card className="p-0 overflow-hidden">
        <div className="p-4 border-b border-slate-200">
          <h2 className="text-lg font-bold text-slate-900">Student Roster</h2>
        </div>
        
        {isLoading ? (
          <div className="p-6">
            <SkeletonList rows={5} />
          </div>
        ) : students.length === 0 ? (
          <div className="p-10">
            <EmptyState title="No students found" message="Import students via CSV to get started." />
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm whitespace-nowrap">
              <thead className="bg-slate-50 text-slate-500">
                <tr>
                  <th className="px-4 py-3 font-medium">Student ID</th>
                  <th className="px-4 py-3 font-medium">Name</th>
                  <th className="px-4 py-3 font-medium">Email</th>
                  <th className="px-4 py-3 font-medium">Gender</th>
                  <th className="px-4 py-3 font-medium">Status</th>
                  <th className="px-4 py-3 font-medium">Joined</th>
                  <th className="px-4 py-3 font-medium text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200">
                {students.map((s) => (
                  <tr key={s.id} className="hover:bg-slate-50">
                    <td className="px-4 py-3 font-medium text-slate-900">{s.studentId}</td>
                    <td className="px-4 py-3 text-slate-700">{s.name}</td>
                    <td className="px-4 py-3 text-slate-600">{s.email}</td>
                    <td className="px-4 py-3 text-slate-600">{s.gender}</td>
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
                          onClick={() => handleResetPassword(s.id)}
                          title="Reset Password"
                          className="p-1.5 text-slate-400 hover:text-brand-600 hover:bg-brand-50 rounded"
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
    </div>
  );
}
