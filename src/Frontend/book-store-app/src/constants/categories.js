import { faStar, faPalette, faSmile } from '@fortawesome/free-solid-svg-icons';

export const dashboardCategories = [
	{ icon: faStar, label: 'Science' },
	{ icon: faPalette, label: 'Art' },
	{ icon: faSmile, label: 'Religion' },
	{ icon: faSmile, label: 'History' },
	{ icon: faSmile, label: 'Geography' },
];

export const categoryOptions = dashboardCategories.map(({ label }) => label);
