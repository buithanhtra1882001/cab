import { useState, useEffect } from 'react';
import { VietQR } from 'vietqr';

const useVietQR = ({ clientID, apiKey }) => {
  const [banks, setBanks] = useState<any>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<any>(null);

  useEffect(() => {
    const vietQR = new VietQR({ clientID, apiKey });

    const fetchBanks = async () => {
      try {
        const response = await vietQR.getBanks();
        setBanks(response.data);
      } catch (err) {
        setError(err);
      } finally {
        setLoading(false);
      }
    };

    fetchBanks();
  }, [clientID, apiKey]);

  return { banks, loading, error };
};

export default useVietQR;
