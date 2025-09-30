document.addEventListener("DOMContentLoaded", () => {
    const apiUrl = "https://localhost:7112/api/customers/Transactions"; // **change to your API endpoint**

    const tableEl = $("#transactions-table");
    let dataTable = null;
    let allTransactions = []; // store full data

    // Initialize date pickers
    flatpickr("#filter-from", { dateFormat: "Y-m-d" });
    flatpickr("#filter-to", { dateFormat: "Y-m-d" });

    // Fetch transactions from API
    async function fetchTransactions() {
        try {
            const resp = await fetch(apiUrl);
            if (!resp.ok) {
                throw new Error(`HTTP ${resp.status}`);
            }
            const json = await resp.json();
            // Expecting an array of transactions
            allTransactions = json;
            populateCustomerFilter(json);
            renderTable(json);
        } catch (err) {
            console.error("Failed to fetch transactions:", err);
        }
    }

    function populateCustomerFilter(transactions) {
        const select = document.getElementById("filter-customer");
        const unique = new Set(transactions.map((t) => t.customerName || t.customerId));
        unique.forEach((c) => {
            const opt = document.createElement("option");
            opt.value = c;
            opt.text = c;
            select.appendChild(opt);
        });
    }

    function renderTable(data) {
        // Destroy existing DataTable if exists
        if (dataTable) {
            dataTable.destroy();
            tableEl.find("tbody").empty();
        }

        const tbody = tableEl.find("tbody");
        data.forEach((t) => {
            const tr = $("<tr></tr>");
            // Format date (assuming ISO date string)
            const dt = new Date(t.date);
            const dtStr = dt.toLocaleDateString(undefined, { year: "numeric", month: "short", day: "numeric" });
            tr.append(`<td>${dtStr}</td>`);

            // Amount with class
            const amtCls = t.type === "Credit" ? "amount-credit" : "amount-debit";
            tr.append(`<td class="${amtCls}">${t.amount.toFixed(2)}</td>`);

            tr.append(`<td>${t.type}</td>`);
            tr.append(`<td>${t.notes || ""}</td>`);
            tbody.append(tr);
        });

        // Initialize DataTable
        dataTable = tableEl.DataTable({
            // optional tweaks
            pageLength: 10,
            order: [[0, "desc"]],
            // we will use the built-in search, but also link our custom filters
        });

        // Apply external search input to table
        $("#search-input").on("input", function () {
            dataTable.search(this.value).draw();
        });
    }

    // Apply filters
    function applyFilters() {
        const cust = document.getElementById("filter-customer").value;
        const type = document.getElementById("filter-type").value;
        const from = document.getElementById("filter-from").value;
        const to = document.getElementById("filter-to").value;

        let filtered = allTransactions;

        if (cust) {
            filtered = filtered.filter((t) => (t.customerName || t.customerId) === cust);
        }
        if (type) {
            filtered = filtered.filter((t) => t.type === type);
        }
        if (from) {
            const dFrom = new Date(from);
            filtered = filtered.filter((t) => new Date(t.date) >= dFrom);
        }
        if (to) {
            const dTo = new Date(to);
            filtered = filtered.filter((t) => new Date(t.date) <= dTo);
        }

        renderTable(filtered);
    }

    // Reset filters
    document.getElementById("btn-reset").addEventListener("click", () => {
        document.getElementById("filter-customer").value = "";
        document.getElementById("filter-type").value = "";
        document.getElementById("filter-from").value = "";
        document.getElementById("filter-to").value = "";
        document.getElementById("search-input").value = "";
        renderTable(allTransactions);
    });

    // Listen to change events on filters
    document.getElementById("filter-customer").addEventListener("change", applyFilters);
    document.getElementById("filter-type").addEventListener("change", applyFilters);
    document.getElementById("filter-from").addEventListener("change", applyFilters);
    document.getElementById("filter-to").addEventListener("change", applyFilters);

    // Initial load
    fetchTransactions();
});
