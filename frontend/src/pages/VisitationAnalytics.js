import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './VisitationAnalytics.css';

function VisitationAnalytics() {
  const navigate = useNavigate();
  
  // Data states
  const [hotels, setHotels] = useState([]);
  const [visitations, setVisitations] = useState([]);
  const [filteredVisitations, setFilteredVisitations] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  
  // Search form states
  const [searchFilters, setSearchFilters] = useState({
    selectedHotels: [],
    monthYear: '',
    onlyLoyalCustomers: false
  });

  useEffect(() => {
    fetchHotels();
    fetchAllVisitations();
  }, []);

  const fetchHotels = async () => {
    try {
      const response = await fetch('http://localhost:5000/api/Hotel');
      if (response.ok) {
        const data = await response.json();
        setHotels(data);
      }
    } catch (err) {
      console.error('Failed to fetch hotels:', err);
    }
  };

  const fetchAllVisitations = async () => {
    setLoading(true);
    try {
      const response = await fetch('http://localhost:5000/api/Visitation');
      if (!response.ok) {
        throw new Error('Failed to fetch visitations');
      }
      const data = await response.json();
      setVisitations(data);
      setFilteredVisitations(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const fetchLoyalCustomers = async (date) => {
    try {
      const response = await fetch(`http://localhost:5000/api/Customer/loyal?date=${date}`);
      if (response.ok) {
        return await response.json();
      }
      return [];
    } catch (err) {
      console.error('Failed to fetch loyal customers:', err);
      return [];
    }
  };

  const fetchVisitationsByDateRange = async (startDate, endDate) => {
    try {
      const response = await fetch(
        `http://localhost:5000/api/Visitation/daterange?startDate=${startDate}&endDate=${endDate}`
      );
      if (response.ok) {
        return await response.json();
      }
      return [];
    } catch (err) {
      console.error('Failed to fetch visitations by date range:', err);
      return [];
    }
  };

  const handleHotelSelection = (hotelId) => {
    setSearchFilters(prev => ({
      ...prev,
      selectedHotels: prev.selectedHotels.includes(hotelId)
        ? prev.selectedHotels.filter(id => id !== hotelId)
        : [...prev.selectedHotels, hotelId]
    }));
  };

  const handleMonthYearChange = (e) => {
    setSearchFilters(prev => ({
      ...prev,
      monthYear: e.target.value
    }));
  };

  const handleLoyalCustomersToggle = () => {
    setSearchFilters(prev => ({
      ...prev,
      onlyLoyalCustomers: !prev.onlyLoyalCustomers
    }));
  };

  const formatMonthYearForAPI = (monthYear) => {
    if (!monthYear) return null;
    const [year, month] = monthYear.split('-');
    return `${year}-${month}-01T00:00:00.000Z`;
  };

  const getDateRangeFromMonthYear = (monthYear) => {
    if (!monthYear) return null;
    
    const [year, month] = monthYear.split('-');
    const startDate = new Date(year, month - 1, 1);
    const endDate = new Date(year, month, 0, 23, 59, 59);
    
    return {
      start: startDate.toISOString(),
      end: endDate.toISOString()
    };
  };

  const handleSearch = async () => {
    setLoading(true);
    setError(null);
    
    try {
      let filteredData = [...visitations];

      // Filter by month/year if specified
      if (searchFilters.monthYear) {
        const dateRange = getDateRangeFromMonthYear(searchFilters.monthYear);
        if (dateRange) {
          const dateFilteredData = await fetchVisitationsByDateRange(dateRange.start, dateRange.end);
          filteredData = dateFilteredData;
        }
      }

      // Filter by selected hotels
      if (searchFilters.selectedHotels.length > 0) {
        filteredData = filteredData.filter(visit => 
          searchFilters.selectedHotels.includes(visit.hotelId)
        );
      }

      // Filter by loyal customers if checkbox is checked
      if (searchFilters.onlyLoyalCustomers) {
        const loyalDate = searchFilters.monthYear 
          ? formatMonthYearForAPI(searchFilters.monthYear)
          : new Date().toISOString();
        
        const loyalCustomers = await fetchLoyalCustomers(loyalDate);
        const loyalCustomerIds = loyalCustomers.map(customer => customer.id);
        
        filteredData = filteredData.filter(visit => 
          loyalCustomerIds.includes(visit.customerId)
        );
      }

      setFilteredVisitations(filteredData);
    } catch (err) {
      setError('Failed to filter visitations: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleCustomerClick = (customerId) => {
    navigate(`/customer/${customerId}`);
  };

  const clearFilters = () => {
    setSearchFilters({
      selectedHotels: [],
      monthYear: '',
      onlyLoyalCustomers: false
    });
    setFilteredVisitations(visitations);
  };

  const formatDisplayDate = (monthYear) => {
    if (!monthYear) return '';
    const [year, month] = monthYear.split('-');
    return `${month}/${year}`;
  };

  return (
    <div className="analytics-container">
      <div className="analytics-header">
        <h1>Visitation Analytics Dashboard</h1>
      </div>

      {error && (
        <div className="error-message">
          <h3>Error</h3>
          <p>{error}</p>
          <p>Make sure the backend server is running on http://localhost:5000</p>
        </div>
      )}

      {/* Search Panel */}
      <div className="search-panel">
        <h2>Search Filters</h2>
        
        {/* Hotel Multi-Select */}
        <div className="filter-group">
          <label>Hotels:</label>
          <div className="hotel-multiselect">
            {hotels.map(hotel => (
              <div key={hotel.id} className="hotel-checkbox">
                <input
                  type="checkbox"
                  id={`hotel-${hotel.id}`}
                  checked={searchFilters.selectedHotels.includes(hotel.id)}
                  onChange={() => handleHotelSelection(hotel.id)}
                />
                <label htmlFor={`hotel-${hotel.id}`}>
                  {hotel.name} - {hotel.city}
                </label>
              </div>
            ))}
          </div>
        </div>

        {/* Month/Year Picker */}
        <div className="filter-group">
          <label htmlFor="monthYear">Month/Year:</label>
          <input
            type="month"
            id="monthYear"
            value={searchFilters.monthYear}
            onChange={handleMonthYearChange}
            placeholder="MM/yyyy"
          />
          {searchFilters.monthYear && (
            <span className="month-display">
              Selected: {formatDisplayDate(searchFilters.monthYear)}
            </span>
          )}
        </div>

        {/* Loyal Customers Checkbox */}
        <div className="filter-group">
          <div className="checkbox-group">
            <input
              type="checkbox"
              id="loyalCustomers"
              checked={searchFilters.onlyLoyalCustomers}
              onChange={handleLoyalCustomersToggle}
            />
            <label htmlFor="loyalCustomers">Only Loyal Customers</label>
          </div>
        </div>

        {/* Action Buttons */}
        <div className="search-actions">
          <button 
            className="btn-primary" 
            onClick={handleSearch}
            disabled={loading}
          >
            {loading ? 'Searching...' : 'Search'}
          </button>
          <button 
            className="btn-secondary" 
            onClick={clearFilters}
          >
            Clear Filters
          </button>
        </div>
      </div>

      {/* Data Grid */}
      <div className="data-grid-section">
        <div className="grid-header">
          <h2>Visitation Results</h2>
          <span className="result-count">
            {filteredVisitations.length} visitation(s) found
          </span>
        </div>

        {loading ? (
          <div className="loading">Loading visitation data...</div>
        ) : (
          <div className="data-grid">
            <table>
              <thead>
                <tr>
                  <th>Index</th>
                  <th>Customer Name</th>
                  <th>Visit Date</th>
                  <th>Hotel Name</th>
                </tr>
              </thead>
              <tbody>
                {filteredVisitations.length === 0 ? (
                  <tr>
                    <td colSpan="4" className="no-data">
                      No visitations found with current filters
                    </td>
                  </tr>
                ) : (
                  filteredVisitations.map((visit, index) => (
                    <tr key={visit.id}>
                      <td>{index + 1}</td>
                      <td>
                        <button
                          className="customer-link"
                          onClick={() => handleCustomerClick(visit.customerId)}
                          title="Click to view customer profile"
                        >
                          {visit.customerName}
                        </button>
                      </td>
                      <td>{new Date(visit.visitDate).toLocaleDateString()}</td>
                      <td>{visit.hotelName}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>

    </div>
  );
}

export default VisitationAnalytics;