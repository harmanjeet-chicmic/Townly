const API_BASE = "https://tonwly.runasp.net/api";

export async function getMarketplaceProperties(page = 1, pageSize = 9) {
  try {
    const res = await fetch(`${API_BASE}/properties/marketplace?page=${page}&pageSize=${pageSize}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!res.ok) {
      throw new Error('Failed to fetch properties');
    }

    const data = await res.json();
    return data; // Return full response with pagination
  } catch (error) {
    console.error("Error fetching marketplace properties:", error);
    throw error;
  }
}

export async function getPropertyById(propertyId) {
  try {
    const res = await fetch(`${API_BASE}/properties/${propertyId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!res.ok) {
      throw new Error('Failed to fetch property details');
    }

    const data = await res.json();
    return data; // Return full response
  } catch (error) {
    console.error("Error fetching property details:", error);
    throw error;
  }
}

export async function getPropertyInvestInfo(propertyId) {
  try {
    const res = await fetch(`${API_BASE}/properties/${propertyId}/investinfo`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!res.ok) {
      throw new Error('Failed to fetch investment info');
    }

    const data = await res.json();
    return data;
  } catch (error) {
    console.error("Error fetching investment info:", error);
    throw error;
  }
}