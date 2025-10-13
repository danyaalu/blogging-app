# Layout Shift Fix - October 13, 2025

## Problem
When navigating between pages (Index, About, PostDetail), the main content body was changing position slightly. The layout would shift because different pages had varying content lengths, and the minimum height was applied inconsistently.

## Solution
Fixed the layout shift by implementing a consistent main content area height across all pages:

### 1. CSS Changes (`wwwroot/css/app.css`)
- Set `html, body { height: 100%; }` to establish proper height inheritance
- Added `main` element styling with fixed minimum height:
  ```css
  main {
      min-height: calc(100vh - 200px);
      display: flex;
      flex-direction: column;
  }
  
  main > .container {
      flex: 1;
      display: flex;
      flex-direction: column;
  }
  ```

### 2. Page Template Updates
Removed inline `min-height: 80vh` and `background-color: #000000` styles from:
- `Pages/Index.cshtml`
- `Pages/PostDetail.cshtml`

These styles are now handled consistently at the layout level through CSS.

## Result
- Main content area now maintains consistent height across all pages
- Layout only varies based on screen width (responsive design)
- Content takes up the entire allocated space
- No more jarring position shifts when navigating between pages
- Footer stays in a consistent position

## Technical Details
- The `calc(100vh - 200px)` accounts for the navbar and footer height
- Flexbox layout ensures content fills available space properly
- All pages now use the same container structure
- Admin pages automatically benefit from the same fix

