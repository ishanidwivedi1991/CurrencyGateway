import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import CurrencyExchange from './CurrencyExchangeRate/CurrencyExchange.UI.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
        <CurrencyExchange />
  </StrictMode>,
)
