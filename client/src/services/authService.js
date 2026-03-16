// const API_BASE = "http://localhost:5168/api/v1/auth";

// export async function requestNonce() {
//   const res = await fetch(`${API_BASE}/wallet/nonce`, {
//     method: "POST",
//     headers: {
//       "Content-Type": "application/json",
//     },
//   });

//   const data = await res.json();
//   return data.data.nonce;
// }

// export async function verifyWallet(payload) {
//   const res = await fetch(`${API_BASE}/wallet/verify`, {
//     method: "POST",
//     headers: {
//       "Content-Type": "application/json",
//     },
//     body: JSON.stringify(payload),
//   });

//   const data = await res.json();
//   return data.data;
// }

const API_BASE = "https://tonwly.runasp.net/api/v1/auth";

export async function requestNonce() {
  const res = await fetch(`${API_BASE}/wallet/nonce`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
  });

  if (!res.ok) {
    throw new Error('Failed to get nonce');
  }

  const data = await res.json();
  return data.data.nonce;
}

export async function verifyWallet(payload) {
  const res = await fetch(`${API_BASE}/wallet/verify`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!res.ok) {
    throw new Error('Failed to verify wallet');
  }

  const data = await res.json();
  return data.data;
}