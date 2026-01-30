(async function () {
  const params = new URLSearchParams(window.location.search);
  const propertyId = params.get("id");

  const data = await apiGet(`/analytics/properties/${propertyId}/trend?hours=7`);

  const labels = data.map(x => new Date(x.snapshotAt).toLocaleTimeString());
  const prices = data.map(x => x.pricePerShare);

  new Chart(document.getElementById("priceChart"), {
    type: "line",
    data: {
      labels,
      datasets: [{
        label: "Price per Share",
        data: prices,
        borderWidth: 2
      }]
    }
  });
})();
