/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{js,jsx,ts,tsx}'],
  darkMode: 'class',
  theme: {
    extend: {
      container: {
        center: true,
      },
      fontFamily: {
        barlow: ['"Barlow Condensed"', 'sans-serif'],
      },
      colors: {
        primary: 'rgb(var(--color-primary) / <alpha-value>)',
        'primary-soft': 'rgb(var(--color-primary-soft) / <alpha-value>)',
        secondery: 'rgb(var(--color-secondery) / <alpha-value>)',
        bgbody: 'rgb(var(--color-bgbody) / <alpha-value>)',
      },
      animation: {
        gradient: 'gradient 5s linear infinite',
      },
      keyframes: {
        gradient: {
          to: { 'background-position': '200% center' },
        },
      },
    },
  },
  // eslint-disable-next-line global-require
  plugins: [require('@tailwindcss/typography'), require('@designbycode/tailwindcss-text-stroke')],
};
