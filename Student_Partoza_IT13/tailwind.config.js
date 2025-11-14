/** @type {import('tailwindcss').Config} */
module.exports = {
 content: [
 './wwwroot/index.html',
 './Components/**/*.{razor,html}',
 './**/*.razor'
 ],
 darkMode: 'class',
 theme: {
 extend: {
 colors: {
 brand: {
 DEFAULT: '#800000',
 dark: '#660000'
 }
 },
 boxShadow: {
 '3xl': '35px 60px -15px rgba(0,0,0,0.3)'
 },
 borderRadius: {
 '3xl': '1.25rem'
 }
 }
 },
 plugins: [
 require('@tailwindcss/forms'),
 require('@tailwindcss/typography')
 ]
};
