import { useCallback, useEffect, useState } from 'react';
import { Location, BlockerFunction, useBlocker, useNavigate } from 'react-router-dom';

interface UseBlockerProps {
  shouldBlock: boolean;
  shouldUnblock: boolean;
  disable?: boolean;
}

export const useBlockerHook = ({ shouldBlock, shouldUnblock, disable }: UseBlockerProps) => {
  const navigate = useNavigate();

  const [showPrompt, setShowPrompt] = useState(false);
  const [lastLocation, setLastLocation] = useState<Location | null>(null);

  const handleBlockedNavigation = useCallback<BlockerFunction>(
    (location) => {
      setLastLocation(location.nextLocation);
      if (shouldBlock && !shouldUnblock && !disable) {
        setShowPrompt(true);
        return true;
      }
      if (shouldUnblock || disable) {
        return false;
      }
      return false;
    },
    [shouldBlock, shouldUnblock, disable],
  );

  const blocker = useBlocker(handleBlockedNavigation);

  useEffect(() => {
    return () => {
      blocker.reset?.();
    };
  }, [blocker]);

  useEffect(() => {
    if (shouldUnblock && lastLocation) {
      navigate(lastLocation.pathname);
    }
  }, [shouldUnblock, blocker.state]);

  return { state: blocker.state, blocker, showPrompt, setShowPrompt, lastLocation };
};
