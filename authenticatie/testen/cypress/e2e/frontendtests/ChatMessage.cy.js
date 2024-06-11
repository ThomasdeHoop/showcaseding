describe('Login and Navigate to Chat Page', () => {
    beforeEach(() => {
        // Visit the login page
        cy.visit('https://localhost:7074/Identity/Account/Login');

        // Fill in email and password fields
        cy.get('input[name="Input.Email"]').type('fortnite@fortnite.fortnite4');
        cy.get('input[name="Input.Password"]').type('!1aaaA');

        // Submit the form
        cy.get('form').submit();

        // Check if redirected to the expected URL
        cy.url().should('eq', 'https://localhost:7074/');
    });

    it('Navigates to Chat Page after successful login', () => {
        // Click on the "Chat" link or button that leads to the chat page
        cy.contains('Chat').click();

        // Check if redirected to the chat page
        cy.url().should('include', '/Home/Chat');
    });
    it('Enters a message, sends it, and checks if it appears', () => {
        cy.contains('Chat').click();
        // Enter a message in the input field
        const message = "Test message";
        cy.get('#messageInput').type(message).should('have.value', message);

        // Click the send button
        cy.get('#sendButton').click();

        // Check if the message appears in the list
        cy.get('#messagesList').contains('li', message).should('exist');
    });
});