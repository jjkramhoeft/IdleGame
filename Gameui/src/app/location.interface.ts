export interface Location 
{
    id: number
    name: string
    description: string
    descriptionPromptByLLM: string
    pictureUri: URL
    point: 
    {
        longitude: number
        latitude: number
    }
    associateRegionBox: {
        lowerLeftPoint: {
            longitude: number
            latitude: number
        }
    }
    locationTypeKey: number
    height: number
    population: number
    slopeValue: number
    climate: {
        name: string
        predominantWindDirection: {
            name: string
        }
        stormFrequency: number
        precipitationAmount: number
        averageTemperature: number
    }
    geologyId: number
    biomId: number
    age: number
    hasHabor: boolean        
    
}