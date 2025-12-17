/** @type {import('tailwindcss').Config} */
export default {
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
      "text-muted": "#78716c",
      "border-color": "#e7e5e4",
    },
  },
},
  plugins: [],
}

