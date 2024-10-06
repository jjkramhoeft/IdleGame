export interface Person 
{
    id: number
    name: string
    description: string
    descriptionPromptByLLM: string
    pictureUri: URL
    state: number
    sexId: number
    raceId: number
    ageId: number
    personAgeId: number
    professionId: number
    hairStyleId: number
    hairColorId: number
    skinColorId: number
    jewelryId: number
    dressId: number
    dressColorId: number
    wealthId: number   
    chips: string[] 
}