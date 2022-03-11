package com.speechsuper;

import junit.framework.Test;
import junit.framework.TestCase;
import junit.framework.TestSuite;

/**
 * Unit test for simple Sample.
 */
public class SampleTest 
    extends TestCase
{
    /**
     * Create the test case
     *
     * @param testName name of the test case
     */
    public SampleTest( String testName )
    {
        super( testName );
    }

    /**
     * @return the suite of tests being tested
     */
    public static Test suite()
    {
        return new TestSuite( SampleTest.class );
    }

    /**
     * Rigourous Test :-)
     */
    public void testSample()
    {
        assertTrue( true );
    }
}
