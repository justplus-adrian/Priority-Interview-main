# Copilot Instructions - Priority Interview Frontend

## Project Overview
This is a React 18 frontend for a Priority Software interview assignment, designed to interface with a .NET Core 8 backend API. The app displays hotel/customer visitation data and serves as a technical assessment platform.

## Architecture & Key Files

### Central Route Configuration Pattern
- **Routes are centrally managed in `src/App.js`** - Add new pages to the `routes` array:
  ```javascript
  const routes = [
    { path: '/', name: 'Home', component: Welcome },
    { path: '/new-page', name: 'New Page', component: NewComponent }
  ];
  ```
- Navigation automatically updates when routes are added to this array
- Components should be placed in `src/pages/` for pages, `src/components/` for reusable components

### API Integration Pattern
- **Backend API runs on `http://localhost:5000`** - hardcoded in components
- OpenAPI spec is embedded in `src/api.json` (comprehensive hotel/customer/visitation API)
- Key endpoints: `/api/Assignment`, `/api/Customer`, `/api/Hotel`, `/api/Visitation`
- Error handling pattern in `Welcome.js` shows standard fetch + state management approach

### Component Structure
- **Layout**: Sidebar navigation (`Navigation.js`) + main content area (`App.js`)
- **Styling**: Component-specific CSS files (e.g., `Navigation.css`, `Welcome.css`)
- **State Management**: Local useState/useEffect pattern, no external state library

## Development Workflows

### Running the Application
```powershell
npm start  # Starts dev server on http://localhost:3000
npm build  # Production build
npm test   # Jest tests
```

### Adding New Features
1. Create component in appropriate folder (`pages/` or `components/`)
2. Add route to `routes` array in `App.js` if it's a page
3. Create corresponding CSS file for styling
4. Use existing API patterns from `Welcome.js` for backend calls

## Project-Specific Conventions

### Text Formatting Pattern
- Rich text formatting in `Welcome.js` shows pattern for handling backend text with:
  - Numbered headings (lines starting with numbers)
  - Bold headings (lines with `**text**`)
  - Paragraph and line break handling

### CSS Architecture
- Global reset and app layout in `App.css`
- Component-specific styles in matching `.css` files
- Responsive design patterns (see `Welcome.css` media queries)
- Color scheme: Primary blue (`#007bff`), sidebar dark (`#2c3e50`)

### Error Handling Pattern
- Standard pattern: loading state → error state → success state
- Error messages include backend connection guidance
- User-friendly error UI with styling from `Welcome.css`

## Backend Integration
- **Expected backend URL**: `http://localhost:5000`
- **Data models**: Customer, Hotel, Visitation, VisitationDetail (see `api.json` schemas)
- **Interview content**: Fetched dynamically from `/api/Assignment` endpoint
- Backend must be running for full functionality - frontend shows connection errors otherwise

## File Organization
```
src/
├── App.js          # Main app with central routing
├── index.js        # React root
├── api.json        # Backend API specification
├── components/     # Reusable UI components
└── pages/          # Page-level components
```

When adding new pages, follow the established patterns in `Welcome.js` for API calls and `Navigation.js` for consistent UI structure.