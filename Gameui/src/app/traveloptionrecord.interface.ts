export interface TravelOptionRecord 
{
    hasPreviouslyVisitedB: boolean
    travelModes: [ number]
    distance: number
    direction: 
    {
        name: string
    }
    departAction: URL
}