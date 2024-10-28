import { jest } from '@jest/globals';
import { handler } from './index.mjs';

// Mock the utils.mjs functions
jest.mock('../../shared/utils.mjs', () => ({
  getSecret: jest.fn(() => Promise.resolve({ username: 'user', password: 'pass' })),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      // Mock database responses
      if (query.includes('SELECT items_owned')) {
        if (values[0] === 'emp_001') return { rows: [{ items_owned: [1, 2, 3], items_equipped: [] }] };
        if (values[0] === 'emp_003') return { rows: [{ items_owned: [], items_equipped: [] }] };
        return { rows: [] };
      }
      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

// Mock event data
const mockEventValid = {
  body: JSON.stringify({
    action: 'equip',
    items: [1, 2], // Items that the user owns
    employee_id: 'emp_001', // Valid employee_id with owned items
  }),
};

const mockEventInvalid = {
  body: JSON.stringify({
    action: 'equip',
    items: [5], // Item not owned by the user
    employee_id: 'emp_003', // Valid employee_id but with no owned items
  }),
};

const mockEventNonExistent = {
  body: JSON.stringify({
    action: 'equip',
    items: [1], // Any item
    employee_id: 'non_existent_emp', // Non-existent employee_id
  }),
};

describe('Equip Handler Tests', () => {
  it('should successfully equip items', async () => {
    const response = await handler(mockEventValid);
    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.message).toContain('Successfully equipped');
  });

  it('should throw an error for an invalid employee_id', async () => {
    const response = await handler(mockEventNonExistent);
    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('User not found with the specified employee_id.');
  });

  it('should throw an error if the user does not own the items', async () => {
    const response = await handler(mockEventInvalid);
    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toContain('User does not own these items');
  });
});
