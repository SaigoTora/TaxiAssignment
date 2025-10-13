import { Box, Flex, Text } from '@chakra-ui/react'
import { HungarianStyle, AuctionStyle } from './MapMarkers'

export default function MapLegend() {
	return (
		<Box
			position='absolute'
			top='8px'
			right='8px'
			bg='white'
			color='black'
			padding='6px 10px'
			borderRadius='md'
			shadow='sm'
			fontSize='12px'
			zIndex={10}
		>
			<Flex align='center' gap='6px' mb='2px'>
				<Box
					w='20px'
					h={`${HungarianStyle.strokeWeight}px`}
					bg={HungarianStyle.color}
				/>
				<Text>Hungarian</Text>
			</Flex>
			<Flex align='center' gap='6px'>
				<Box
					w='20px'
					h={`${AuctionStyle.strokeWeight}px`}
					bg={AuctionStyle.color}
				/>
				<Text>Auction</Text>
			</Flex>
		</Box>
	)
}
