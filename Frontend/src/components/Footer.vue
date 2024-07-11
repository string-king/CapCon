<template>
  <footer class="footer">
    <div class="container">
      <p>&copy; 2024 - CapCon. All rights reserved.</p>
      <div class="social-media">
        <a href="#" target="_blank">Facebook</a>
        <a href="#" target="_blank">Twitter</a>
        <a href="#" target="_blank">LinkedIn</a>
      </div>
    </div>
  </footer>
</template>

<style scoped>
/* Footer styles */
.footer {
  background: var(--primary-color);
  color: var(--text-color);
  padding: 1rem 0;
  font-family: 'Roboto', sans-serif;
  border-top: 1px solid var(--secondary-color);
  width: 100%; /* Ensure full width */
  position: fixed; /* Fixed position */
  bottom: 0; /* Stick to the bottom */
  left: 0; /* Align to the left */
}

.footer .container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.footer p {
  margin: 0;
}

.social-media {
  display: flex;
}

.social-media a {
  color: var(--text-color);
  text-decoration: none;
  margin-right: 10px;
}

.social-media a:hover {
  color: var(--hover-color);
}
</style>
