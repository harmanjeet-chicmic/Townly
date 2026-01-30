(async function () {
  try {
    console.log("Fetching marketplace properties...");

    const response = await apiGet("/properties/marketplace?page=1&pageSize=9");
    const properties = response.items ?? response;

    console.log("Properties:", properties);

    const container = document.getElementById("properties");

    properties.forEach(p => {
      const div = document.createElement("div");
      div.className = "card";

      div.innerHTML = `
        <h3>${p.name}</h3>
        <p>${p.location}</p>
        <p>Price/Share: ${p.pricePerUnit}</p>
        <p>Total Value: ${p.totalValue}</p>
        <a href="property.html?id=${p.id}">View Analytics</a>
      `;

      container.appendChild(div);
    });
  } catch (e) {
    console.error("Marketplace error:", e);
  }
})();
