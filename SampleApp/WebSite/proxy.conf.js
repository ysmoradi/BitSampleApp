const PROXY_CONFIG = [
  {
    context: ["/api", "/odata", "/signalr", "/InvokeLogin", "/InvokeLogout", "/Metadata", "/Files", "/SignIn", "/SignOut", "/ClientAppProfile", "/core", "/jobs"],
    target: "http://localhost:17563",
    secure: false
  }
];

module.exports = PROXY_CONFIG;
