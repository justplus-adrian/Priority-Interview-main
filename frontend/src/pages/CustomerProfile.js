import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import './CustomerProfile.css';

function CustomerProfile() {
  const { id } = useParams();
  const [customer, setCustomer] = useState(null);
  const [hotels, setHotels] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  
  // Form states
  const [formData, setFormData] = useState({
    name: '',
    email: ''
  });
  
  // Visit registration state
  const [visitData, setVisitData] = useState({
    hotelId: '',
    visitDate: new Date().toISOString().split('T')[0]
  });

  useEffect(() => {
    if (id) {
      // Fetch customer data if ID is provided
      fetchCustomer(id);
    }
    // Always fetch hotels for visit registration
    fetchHotels();
  }, [id]);

  const fetchCustomer = async (customerId) => {
    setLoading(true);
    try {
      const response = await fetch(`http://localhost:5000/api/Customer/${customerId}`);
      if (!response.ok) {
        throw new Error('Customer not found');
      }
      const data = await response.json();
      setCustomer(data);
      setFormData({
        name: data.name || '',
        email: data.email || ''
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

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

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleVisitInputChange = (e) => {
    const { name, value } = e.target;
    setVisitData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleCreateProfile = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const response = await fetch('http://localhost:5000/api/Customer/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          ...formData,
          registrationDate: new Date().toISOString(),
          totalPurchases: 0
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to create customer profile');
      }

      const newCustomer = await response.json();
      setSuccess(`Profile created successfully! Customer ID: ${newCustomer.id}`);
      setCustomer(newCustomer);
      
      // Update URL to show the new customer
      window.history.pushState({}, '', `/customer/${newCustomer.id}`);
      
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleRegisterVisit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      const response = await fetch('http://localhost:5000/api/Visitation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          customerId: parseInt(id),
          hotelId: parseInt(visitData.hotelId),
          visitDate: new Date(visitData.visitDate).toISOString()
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to register visit');
      }

      const visitResult = await response.json();
      setSuccess(`Visit registered successfully! Visit ID: ${visitResult.id}`);
      
      // Reset visit form
      setVisitData({
        hotelId: '',
        visitDate: new Date().toISOString().split('T')[0]
      });
      
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading && !customer) {
    return (
      <div className="customer-profile-container">
        <div className="loading">Loading customer information...</div>
      </div>
    );
  }

  return (
    <div className="customer-profile-container">
      <div className="profile-header">
        <h1>{id ? `Customer Profile ${id}` : 'Customer Registration'}</h1>
      </div>

      {error && (
        <div className="error-message">
          <h3>Error</h3>
          <p>{error}</p>
          <p>Make sure the backend server is running on http://localhost:5000</p>
        </div>
      )}

      {success && (
        <div className="success-message">
          <p>{success}</p>
        </div>
      )}

      {/* Customer Information Section */}
      <div className="profile-content">
        {id && customer ? (
          // Existing customer view
          <div className="customer-info">
            <h2>Customer Information</h2>
            <div className="info-grid">
              <div className="info-item">
                <label>Name:</label>
                <span>{customer.name}</span>
              </div>
              <div className="info-item">
                <label>Email:</label>
                <span>{customer.email}</span>
              </div>
              <div className="info-item">
                <label>Registration Date:</label>
                <span>{new Date(customer.registrationDate).toLocaleDateString()}</span>
              </div>
            </div>
          </div>
        ) : (
          // Customer registration form
          <div className="customer-form">
            <h2>Create Customer Profile</h2>
            <form onSubmit={handleCreateProfile}>
              <div className="form-group">
                <label htmlFor="name">Name:</label>
                <input
                  type="text"
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="email">Email:</label>
                <input
                  type="email"
                  id="email"
                  name="email"
                  value={formData.email}
                  onChange={handleInputChange}
                  required
                />
              </div>
              <button type="submit" className="btn-primary" disabled={loading}>
                {loading ? 'Creating Profile...' : 'Create Profile'}
              </button>
            </form>
          </div>
        )}

        {/* Visit Registration Section - Only show if customer exists */}
        {(id && customer) && (
          <div className="visit-registration">
            <h2>Register Visit</h2>
            <form onSubmit={handleRegisterVisit}>
              <div className="form-group">
                <label htmlFor="hotelId">Select Hotel:</label>
                <select
                  id="hotelId"
                  name="hotelId"
                  value={visitData.hotelId}
                  onChange={handleVisitInputChange}
                  required
                >
                  <option value="">Choose a hotel...</option>
                  {hotels.map(hotel => (
                    <option key={hotel.id} value={hotel.id}>
                      {hotel.name} 
                    </option>
                  ))}
                </select>
              </div>
              <div className="form-group">
                <label htmlFor="visitDate">Visit Date:</label>
                <input
                  type="date"
                  id="visitDate"
                  name="visitDate"
                  value={visitData.visitDate}
                  onChange={handleVisitInputChange}
                  required
                />
              </div>
              <button type="submit" className="btn-secondary" disabled={loading}>
                {loading ? 'Registering Visit...' : 'Register Visit'}
              </button>
            </form>
          </div>
        )}
      </div>


    </div>
  );
}

export default CustomerProfile;