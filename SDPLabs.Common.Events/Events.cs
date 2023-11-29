namespace SDPLabs.Common.Events;

public record CarMovedEvent(long Id, double Distance);
public record CarReceivedDangerousMileage(long CarId, double TotalMileage);
public record CarReceivedCriticalMileage(long CarId, double TotalMileage);
