window.onload = function () {
  window.ui = SwaggerUIBundle({
    // url: "https://localhost:5001/swagger/v1/swagger.json",
    configUrl: `./swagger-config.json`,
    dom_id: "#swagger-ui",
    deepLinking: true,
    presets: [SwaggerUIBundle.presets.apis, SwaggerUIStandalonePreset],
    plugins: [SwaggerUIBundle.plugins.DownloadUrl],
    layout: "StandaloneLayout",
  });
};
