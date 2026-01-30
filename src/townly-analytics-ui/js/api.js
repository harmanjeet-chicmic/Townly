async function apiGet(url, auth = false) {
  const headers = {};

  if (auth) {
    headers["Authorization"] = `Bearer ${AUTH_TOKEN}`;
  }

  const res = await fetch(`${API_BASE_URL}${url}`, { headers });
  if (!res.ok) throw new Error("API error");

  return res.json();
}
