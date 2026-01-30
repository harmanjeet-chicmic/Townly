(async function () {
  const summary = await apiGet("/investments/me/summary", true);
  const trend = await apiGet("/analytics/portfolio/me/trend?hours=7", true);

  document.getElementById("summary").innerHTML = `
    <p>Total Invested: ${summary.totalInvested}</p>
    <p>Properties: ${summary.propertiesCount}</p>
    <p>Shares Owned: ${summary.totalSharesOwned}</p>
  `;

  const labels = trend.map(x => new Date(x.snapshotAt).toLocaleTimeString());
  const values = trend.map(x => x.portfolioValue);

  new Chart(document.getElementById("portfolioChart"), {
    type: "line",
    data: {
      labels,
      datasets: [{
        label: "Portfolio Value",
        data: values,
        borderWidth: 2
      }]
    }
  });
})();
