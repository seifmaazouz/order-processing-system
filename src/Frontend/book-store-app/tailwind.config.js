/** @type {import('tailwindcss').Config} */
export default {
    darkMode: 'class', // Use class-based dark mode (only applies when 'dark' class is on HTML)
    content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
  extend: {
    colors: {
      primary: "#ea580c",
      "primary-hover": "#c2410c",
      "background-light": "#fdfbf7",
      "background-dark": "#23220f",
      "text-main": "#292524",
      "text-main-light": "#292524",
      "text-main-dark": "#fdfbf7",
      "text-muted": "#78716c",
      "border-color": "#e7e5e4",
      "surface-light": "#ffffff",
      "surface-dark": "#1f1f1f",
    },
  },
},
  plugins: [],
}

