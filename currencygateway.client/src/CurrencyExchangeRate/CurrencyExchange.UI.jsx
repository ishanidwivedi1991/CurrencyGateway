import React, { useEffect, useState } from 'react';
import axios from 'axios';
import '../App.css';

//const App = () => {
//    const [exchangeRates, setExchangeRates] = useState([]);
//    const [error, setError] = useState(null);
//    const [prevRates, setPrevRates] = useState({});

//    useEffect(() => {
//        const fetchData = async () => {
//            try {
//                const response = await axios.get('https://localhost:7181/api/currencyexchange/rates');
//                const rates = response.data.rates;
//                const rateArray = Object.entries(rates).map(([currency, rate]) => ({
//                    currency,
//                    rate,
//                }));
//                setExchangeRates(rateArray);
//            } catch (err) {
//                setError('Failed to fetch exchange rates.');
//            }
//        };

//        fetchData();
//        const interval = setInterval(fetchData, 30000); // Update every 30 seconds
//        return () => clearInterval(interval);
//    }, []);

//    useEffect(() => {
//        if (exchangeRates.length) {
//            setPrevRates(
//                exchangeRates.reduce((acc, { currency, rate }) => {
//                    acc[currency] = rate;
//                    return acc;
//                }, {})
//            );
//        }
//    }, [exchangeRates]);

//    const getRowClass = (currency, rate) => {
//        if (!prevRates[currency]) return '';
//        if (rate > prevRates[currency]) return 'positive'; // Rate has increased
//        if (rate < prevRates[currency]) return 'negative'; // Rate has decreased
//        return ''; // Rate is unchanged
//    };

//    return (
//        <div className="App">
//            <h1>Currency Exchange Rates</h1>
//            {error ? (
//                <p>{error}</p>
//            ) : (
//                <table>
//                    <thead>
//                        <tr>
//                            <th>Currency</th>
//                            <th>Rate</th>
//                        </tr>
//                    </thead>
//                    <tbody>
//                        {exchangeRates.map(({ currency, rate }) => (
//                            <tr key={currency} className={getRowClass(currency, rate)}>
//                                <td>{currency}</td>
//                                <td>{rate}</td>
//                            </tr>
//                        ))}
//                    </tbody>
//                </table>
//            )}
//        </div>
//    );
//};

//export default App;

const API_URL = "https://localhost:7181/api/currencyexchange/rates";

const CurrencyExchangeApp = () => {
    const [exchangeRates, setExchangeRates] = useState({});
    const [prevRates, setPrevRates] = useState({});
    const [error, setError] = useState(null);

    // Function to fetch exchange rates
    const fetchExchangeRates = async () => {
        try {
            const response = await fetch(API_URL);
            if (!response.ok) {
                throw new Error("Failed to fetch exchange rates");
            }
            const data = await response.json();

            // Update previous rates before setting new ones
            setPrevRates(exchangeRates);
            setExchangeRates(data.rates);
        } catch (err) {
            setError(err.message);
        }
    };

    // Fetch exchange rates every 30 seconds
    useEffect(() => {
        fetchExchangeRates();
        const interval = setInterval(fetchExchangeRates, 30000); // Fetch every 30 seconds
        return () => clearInterval(interval); // Cleanup interval on component unmount
    }, []);

    // Function to determine row class based on rate changes
    const getRowClass = (currency, rate) => {
        if (!prevRates[currency]) return ""; // No previous rate to compare
        if (rate > prevRates[currency]) return "positive";
        if (rate < prevRates[currency]) return "negative";
        return ""; // Rate unchanged
    };

    const calculatePercentageChange = (currency, rate) => {
        if (!prevRates[currency]) return "N/A"; // No previous rate to calculate percentage
        const previousRate = prevRates[currency];
        const percentageChange = ((rate - previousRate) / previousRate) * 100;
        return `${percentageChange.toFixed(2)}%`;
    };

    return (
        <div>
            <h1>Currency Exchange Rates</h1>
            {error && <p className="error">{error}</p>}
            <table>
                <thead>
                    <tr>
                        <th>Currency</th>
                        <th>Exchange Rate</th>
                        <th>Percentage Change</th>
                    </tr>
                </thead>
                <tbody>
                    {Object.keys(exchangeRates).map((currency) => (
                        <tr key={currency} className={getRowClass(currency, exchangeRates[currency])}>
                            <td>{currency}</td>
                            <td>{exchangeRates[currency]}</td>
                            <td>{calculatePercentageChange(currency, exchangeRates[currency])}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default CurrencyExchangeApp;
