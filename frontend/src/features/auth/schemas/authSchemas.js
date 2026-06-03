export const loginRules = {
  email: {
    required: "Email is required"
  },
  password: {
    required: "Password is required"
  }
};

export const registerRules = {
  name: {
    required: "Name is required",
    minLength: { value: 2, message: "Name must be at least 2 characters" }
  },
  email: {
    required: "Email is required",
    pattern: { value: /^\S+@\S+\.\S+$/, message: "Enter a valid email" }
  },
  password: {
    required: "Password is required",
    minLength: { value: 6, message: "Use at least 6 characters" }
  }
};
